using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.CommonMembers;
using Mono.Cecil;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApplicationPatcher.Core.Factories {
	public class CommonAssemblyFactory {
		private const string codeBasePrefix = "file:///";

		private readonly string[] additionalLoadAssemblyNames;

		public CommonAssemblyFactory(params string[] additionalLoadAssemblyNames) {
			this.additionalLoadAssemblyNames = additionalLoadAssemblyNames;
		}

		public virtual CommonAssembly Create(string assemblyPath) {
			using (CurrentDirectoryHelper.From(Path.GetDirectoryName(Path.GetFullPath(assemblyPath)))) {
				var assemblyName = Path.GetFileName(assemblyPath);

				var symbolStorePath = Path.ChangeExtension(assemblyName, "pdb");
				var haveSymbolStore = File.Exists(symbolStorePath);

				var foundAssemblyFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
					.Select(filePath => new { Path = filePath, Extension = Path.GetExtension(filePath) })
					.Where(file =>
						string.Equals(file.Extension, ".exe", StringComparison.InvariantCultureIgnoreCase) ||
						string.Equals(file.Extension, ".dll", StringComparison.InvariantCultureIgnoreCase))
					.GroupBy(file => Path.GetFileNameWithoutExtension(file.Path))
					.ToDictionary(group => group.Key, group => group.Single().Path);

				var mainReflectionAssembly = ReadMainReflectionAssembly(assemblyName, symbolStorePath, haveSymbolStore);
				var referencedReflectionAssemblies = ReadReferencedReflectionAssemblies(mainReflectionAssembly, foundAssemblyFiles);

				referencedReflectionAssemblies
					.Select(assembly => new { assembly.GetName().Name, assembly.CodeBase })
					.Where(assembly => !foundAssemblyFiles.ContainsKey(assembly.Name))
					.ForEach(assembly => foundAssemblyFiles[assembly.Name] = assembly.CodeBase.Substring(codeBasePrefix.Length));

				var mainMonoCecilAssembly = ReadMainMonoCecilAssembly(assemblyName, haveSymbolStore);
				var referencedMonoCecilAssemblies = ReadReferencedMonoCecilAssemblies(mainMonoCecilAssembly, foundAssemblyFiles);

				return new CommonAssembly(mainReflectionAssembly, referencedReflectionAssemblies, mainMonoCecilAssembly, referencedMonoCecilAssemblies, haveSymbolStore);
			}
		}

		private static Assembly ReadMainReflectionAssembly(string assemblyName, string symbolStorePath, bool haveSymbolStore) {
			var loadedAssembly = FindLoadedAssembly(Path.GetFileNameWithoutExtension(assemblyName));
			if (loadedAssembly != null)
				return loadedAssembly;

			var rawAssembly = File.ReadAllBytes(assemblyName);

			if (!haveSymbolStore)
				return Assembly.Load(rawAssembly);

			var rawSymbolStore = File.ReadAllBytes(symbolStorePath);
			File.Delete(symbolStorePath);

			var mainAssembly = Assembly.Load(rawAssembly, rawSymbolStore);
			File.WriteAllBytes(symbolStorePath, rawSymbolStore);

			return mainAssembly;
		}

		private Assembly[] ReadReferencedReflectionAssemblies(Assembly mainReflectionAssembly, IDictionary<string, string> foundAssemblyFiles) {
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => FindLoadedAssembly(new AssemblyName(args.Name).Name) ??
				(foundAssemblyFiles.TryGetValue(new AssemblyName(args.Name).Name, out var assemblyFile) ? Assembly.Load(File.ReadAllBytes(assemblyFile)) : null);

			var referencedReflectionAssemblies = mainReflectionAssembly.GetReferencedAssemblies().Select(Assembly.Load).ToList();

			foreach (var assemblyName in additionalLoadAssemblyNames.Where(assemblyName => referencedReflectionAssemblies.All(assembly => assembly.GetName().Name != assemblyName))) {
				var readedAssembly = foundAssemblyFiles.TryGetValue(assemblyName, out var assemblyFile)
					? Assembly.Load(File.ReadAllBytes(assemblyFile))
					: throw new InvalidOperationException($"Not found additional assembly '{assemblyName}' for reflection reader");

				foreach (var referencedAssembly in readedAssembly.GetReferencedAssemblies().Select(Assembly.Load).Where(assembly => !foundAssemblyFiles.ContainsKey(assembly.GetName().Name)))
					foundAssemblyFiles[referencedAssembly.GetName().Name] = referencedAssembly.CodeBase.Substring(codeBasePrefix.Length);

				referencedReflectionAssemblies.Add(readedAssembly);
			}

			return referencedReflectionAssemblies.ToArray();
		}

		private static Assembly FindLoadedAssembly(string assemblyName) {
			return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);
		}

		private static AssemblyDefinition ReadMainMonoCecilAssembly(string assemblyName, bool haveSymbolStore) {
			return AssemblyDefinition.ReadAssembly(assemblyName, new ReaderParameters { ReadSymbols = haveSymbolStore, InMemory = true });
		}

		private AssemblyDefinition[] ReadReferencedMonoCecilAssemblies(AssemblyDefinition mainMonoCecilAssembly, IDictionary<string, string> foundAssemblyFiles) {
			var referencedMonoCecilAssemblies = mainMonoCecilAssembly.MainModule.AssemblyReferences
				.Select(assembly => foundAssemblyFiles.TryGetValue(assembly.Name, out var assemblyFile)
					? AssemblyDefinition.ReadAssembly(assemblyFile)
					: throw new InvalidOperationException($"Not found assembly '{assembly.Name}' for mono cecil reader"))
				.ToList();

			referencedMonoCecilAssemblies.AddRange(additionalLoadAssemblyNames
				.Except(referencedMonoCecilAssemblies.Select(assembly => assembly.Name.Name))
				.Select(assemblyName => foundAssemblyFiles.TryGetValue(assemblyName, out var assemblyFile)
					? AssemblyDefinition.ReadAssembly(assemblyFile)
					: throw new InvalidOperationException($"Not found additional assembly '{assemblyName}' for mono cecil reader")));

			return referencedMonoCecilAssemblies.ToArray();
		}

		public virtual void Save(CommonAssembly commonAssembly, string assemblyPath, string signaturePath = null) {
			var strongNameKeyPair = signaturePath.IsNullOrEmpty() ? null : new StrongNameKeyPair(File.ReadAllBytes(signaturePath));
			commonAssembly.MonoCecil.Write(assemblyPath, new WriterParameters { WriteSymbols = commonAssembly.HaveSymbolStore, StrongNameKeyPair = strongNameKeyPair });
		}
	}
}