using System;
using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	public class MonoCecilSelectedTypesPatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly Log log;

		public MonoCecilSelectedTypesPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			log.Info("Patching selected types...");
			var foundedSelectedPatchingTypes = assembly.TypesFromThisAssembly
				.Where(type => applicationPatcherSelfConfiguration.MonoCecilSelectedPatchingTypeFullNames.Contains(type.FullName))
				.ToArray();

			if (!foundedSelectedPatchingTypes.Any()) {
				log.Info("Not found selected types");
				return PatchResult.Succeeded;
			}

			log.Debug("Selected types found:", foundedSelectedPatchingTypes.Select(viewModel => viewModel.FullName).OrderBy(fullName => fullName));

			foreach (var type in foundedSelectedPatchingTypes) {
				log.Info($"Loading type '{type.FullName}'...");
				type.Load();
				log.Info($"Type '{type.FullName}' was loaded");

				log.Info($"Patching type '{type.FullName}'...");

				type.MonoCecil.IsSealed = false;

				var emptyConstructor = type.GetEmptyConstructor();
				if (emptyConstructor != null)
					emptyConstructor.MonoCecil.IsAssembly = true;
				else {
					const MethodAttributes emptyConstructorMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
					var objectEmptyConstructorMethod = assembly.MonoCecil.MainModule.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes));

					var emptyConstructorMethod = new MethodDefinition(".ctor", emptyConstructorMethodAttributes, assembly.MonoCecil.MainModule.TypeSystem.Void);
					emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Call, objectEmptyConstructorMethod));
					emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

					type.MonoCecil.Methods.Add(emptyConstructorMethod);
				}

				foreach (var property in type.Properties) {
					if (property.MonoCecil.GetMethod != null)
						property.MonoCecil.GetMethod.IsVirtual = true;
					if (property.MonoCecil.SetMethod != null)
						property.MonoCecil.SetMethod.IsVirtual = true;
				}

				log.Info($"Type '{type.FullName}' was patched");
			}

			foundedSelectedPatchingTypes.ForEach(type => type.Load().MonoCecil.IsSealed = false);
			log.Info("Sealed types was patched");
			return PatchResult.Succeeded;
		}
	}
}