using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Core.Factories {
	public class CommonAssemblyFactory {
		public CommonAssembly Create([NotNull] string assemblyPath) {
			var reflectionAssembly = CreateReflectionAssembly(assemblyPath);
			var monoCecilAssembly = CreateMonoCecilAssembly(assemblyPath);

			return new CommonAssembly(reflectionAssembly, monoCecilAssembly);
		}

		private static Assembly[] CreateReflectionAssembly([NotNull] string assemblyPath) {
			var mainAssembly = ReadMainReflectionAssembly(assemblyPath);

			var foundedAssemblyFiles = Directory.GetFiles(Directory.GetCurrentDirectory())
				.GroupBy(Path.GetFileNameWithoutExtension)
				.ToDictionary(group => group.Key, group => group.SingleOrDefault(path => Path.GetExtension(path) == ".exe" || Path.GetExtension(path) == ".dll"));

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
				foundedAssemblyFiles.TryGetValue(new AssemblyName(args.Name).Name, out var assemblyFile) ? Assembly.Load(File.ReadAllBytes(assemblyFile)) : null;

			return new[] { mainAssembly }.Concat(mainAssembly.GetReferencedAssemblies().Select(Assembly.Load)).ToArray();
		}
		private static Assembly ReadMainReflectionAssembly([NotNull] string assemblyPath) {
			var symbolStorePath = Path.ChangeExtension(assemblyPath, "pdb");
			var rawAssembly = File.ReadAllBytes(assemblyPath);

			if (!File.Exists(symbolStorePath))
				return Assembly.Load(rawAssembly);

			var rawSymbolStore = File.ReadAllBytes(symbolStorePath);
			File.Delete(symbolStorePath);

			var mainAssembly = Assembly.Load(rawAssembly, rawSymbolStore);
			File.WriteAllBytes(symbolStorePath, rawSymbolStore);

			return mainAssembly;
		}

		private static AssemblyDefinition CreateMonoCecilAssembly([NotNull] string assemblyPath) {
			return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadSymbols = true });
		}

		public virtual void Save(CommonAssembly monoCecilAssembly, [NotNull] string assemblyPath) {
			monoCecilAssembly.MonoCecilAssembly.Write(assemblyPath, new WriterParameters { WriteSymbols = true });
		}
	}
}