﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Types.Common;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Factories {
	public class CommonAssemblyFactory {
		public CommonAssembly Create(string assemblyPath) {
			var symbolStorePath = Path.ChangeExtension(assemblyPath, "pdb");
			var haveSymbolStore = !string.IsNullOrEmpty(symbolStorePath) && File.Exists(symbolStorePath);

			var foundedAssemblyFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
				.Select(filePath => new { Path = filePath, Extension = Path.GetExtension(filePath) })
				.Where(file =>
					string.Equals(file.Extension, ".exe", StringComparison.InvariantCultureIgnoreCase) ||
					string.Equals(file.Extension, ".dll", StringComparison.InvariantCultureIgnoreCase))
				.GroupBy(file => Path.GetFileNameWithoutExtension(file.Path))
				.ToDictionary(group => group.Key, group => group.Single().Path);

			var mainReflectionAssembly = ReadMainReflectionAssembly(assemblyPath, symbolStorePath, haveSymbolStore);
			var referencedReflectionAssemblies = ReadReferencedReflectionAssemblies(mainReflectionAssembly, foundedAssemblyFiles);

			const string codeBasePrefix = "file:///";
			referencedReflectionAssemblies
				.Select(assembly => new { assembly.GetName().Name, assembly.CodeBase })
				.Where(assembly => !foundedAssemblyFiles.ContainsKey(assembly.Name))
				.ForEach(assembly => foundedAssemblyFiles[assembly.Name] = assembly.CodeBase.Substring(codeBasePrefix.Length));

			var mainMonoCecilAssembly = ReadMainMonoCecilAssembly(assemblyPath, haveSymbolStore);
			var referencedMonoCecilAssemblies = ReadReferencedMonoCecilAssemblies(mainMonoCecilAssembly, foundedAssemblyFiles);

			return new CommonAssembly(mainReflectionAssembly, referencedReflectionAssemblies, mainMonoCecilAssembly, referencedMonoCecilAssemblies, haveSymbolStore);
		}

		private static Assembly ReadMainReflectionAssembly(string assemblyPath, string symbolStorePath, bool haveSymbolStore) {
			var loadedAssembly = FindLoadedAssembly(Path.GetFileNameWithoutExtension(assemblyPath));
			if (loadedAssembly != null)
				return loadedAssembly;

			var rawAssembly = File.ReadAllBytes(assemblyPath);

			if (!haveSymbolStore)
				return Assembly.Load(rawAssembly);

			var rawSymbolStore = File.ReadAllBytes(symbolStorePath);
			File.Delete(symbolStorePath);

			var mainAssembly = Assembly.Load(rawAssembly, rawSymbolStore);
			File.WriteAllBytes(symbolStorePath, rawSymbolStore);

			return mainAssembly;
		}

		private static Assembly[] ReadReferencedReflectionAssemblies(Assembly mainReflectionAssembly, Dictionary<string, string> foundedAssemblyFiles) {
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => FindLoadedAssembly(new AssemblyName(args.Name).Name) ??
				(foundedAssemblyFiles.TryGetValue(new AssemblyName(args.Name).Name, out var assemblyFile) ? Assembly.Load(File.ReadAllBytes(assemblyFile)) : null);

			return mainReflectionAssembly.GetReferencedAssemblies().Select(Assembly.Load).ToArray();
		}

		private static Assembly FindLoadedAssembly(string assemblyName) {
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
		}

		private static AssemblyDefinition ReadMainMonoCecilAssembly(string assemblyPath, bool haveSymbolStore) {
			return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadSymbols = haveSymbolStore, InMemory = true });
		}

		private static AssemblyDefinition[] ReadReferencedMonoCecilAssemblies(AssemblyDefinition mainMonoCecilAssembly, Dictionary<string, string> foundedAssemblyFiles) {
			return mainMonoCecilAssembly.MainModule.AssemblyReferences
				.Select(assembly => foundedAssemblyFiles.TryGetValue(assembly.Name, out var assemblyFile)
					? AssemblyDefinition.ReadAssembly(assemblyFile)
					: throw new Exception($"Not found assembly {assembly.Name} for mono cecil reader"))
				.ToArray();
		}

		public virtual void Save(CommonAssembly commonAssembly, string assemblyPath) {
			commonAssembly.MainMonoCecilAssembly.Write(assemblyPath, new WriterParameters { WriteSymbols = commonAssembly.HaveSymbolStore });
		}
	}
}