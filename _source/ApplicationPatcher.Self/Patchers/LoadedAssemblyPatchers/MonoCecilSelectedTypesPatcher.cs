using System;
using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	[UsedImplicitly]
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
				type.Load();
				type.MonoCecilType.IsSealed = false;

				var constructorMethod = type.GetConstructor(new Type[0]);
				if (constructorMethod != null) {
					constructorMethod.MonoCecilConstructor.IsPublic = true;
					constructorMethod.MonoCecilConstructor.IsPrivate = false;
				}
				else {
					var objectType = assembly.GetCommonType(typeof(object)).Load();
					var objectConstructorMethod = objectType.GetConstructor(new Type[0]).Load();
					var a = assembly.MainMonoCecilAssembly.MainModule.ImportReference(objectConstructorMethod.MonoCecilConstructor);

					var methodDefinition = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, assembly.MainMonoCecilAssembly.MainModule.TypeSystem.Void);
					methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Call, a));
					methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

					type.MonoCecilType.Methods.Add(methodDefinition);
				}

				foreach (var property in type.Properties) {
					if (property.MonoCecilProperty.GetMethod != null)
						property.MonoCecilProperty.GetMethod.IsVirtual = true;
					if (property.MonoCecilProperty.SetMethod != null)
						property.MonoCecilProperty.SetMethod.IsVirtual = true;
				}
			}

			foundedSelectedPatchingTypes.ForEach(type => type.Load().MonoCecilType.IsSealed = false);
			log.Info("Sealed types was patched");
			return PatchResult.Succeeded;
		}
	}
}