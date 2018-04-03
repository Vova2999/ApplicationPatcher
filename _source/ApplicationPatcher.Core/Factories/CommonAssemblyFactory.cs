using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Factories {
	public class CommonAssemblyFactory {
		public CommonAssembly Create(string assemblyPath) {
			var symbolStorePath = Path.ChangeExtension(assemblyPath, "pdb");
			var haveSymbolStore = !string.IsNullOrEmpty(symbolStorePath) && File.Exists(symbolStorePath);

			var reflectionAssembly = CreateReflectionAssembly(assemblyPath, symbolStorePath, haveSymbolStore);
			var monoCecilAssembly = CreateMonoCecilAssembly(assemblyPath, haveSymbolStore);

			return new CommonAssembly(haveSymbolStore, reflectionAssembly, monoCecilAssembly).Load();
		}

		private static Assembly[] CreateReflectionAssembly(string assemblyPath, string symbolStorePath, bool haveSymbolStore) {
			var mainAssembly = ReadMainReflectionAssembly(assemblyPath, symbolStorePath, haveSymbolStore);

			var foundedAssemblyFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
				.GroupBy(Path.GetFileNameWithoutExtension)
				.ToDictionary(group => group.Key, group => group.SingleOrDefault(path => Path.GetExtension(path) == ".exe" || Path.GetExtension(path) == ".dll"));

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
				foundedAssemblyFiles.TryGetValue(new AssemblyName(args.Name).Name, out var assemblyFile) ? Assembly.Load(File.ReadAllBytes(assemblyFile)) : null;

			return new[] { mainAssembly }.Concat(mainAssembly.GetReferencedAssemblies().Select(Assembly.Load)).ToArray();
		}
		private static Assembly ReadMainReflectionAssembly([NotNull] string assemblyPath, string symbolStorePath, bool haveSymbolStore) {
			var rawAssembly = File.ReadAllBytes(assemblyPath);

			if (!haveSymbolStore)
				return Assembly.Load(rawAssembly);

			var rawSymbolStore = File.ReadAllBytes(symbolStorePath);
			File.Delete(symbolStorePath);

			var mainAssembly = Assembly.Load(rawAssembly, rawSymbolStore);
			File.WriteAllBytes(symbolStorePath, rawSymbolStore);

			return mainAssembly;
		}

		private static AssemblyDefinition CreateMonoCecilAssembly(string assemblyPath, bool haveSymbolStore) {
			return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadSymbols = haveSymbolStore, InMemory = true });
		}

		public virtual void Save(CommonAssembly commonAssembly, string assemblyPath) {
			commonAssembly.MonoCecilAssembly.Write(assemblyPath, new WriterParameters { WriteSymbols = commonAssembly.HaveSymbolStore });
		}
	}
}