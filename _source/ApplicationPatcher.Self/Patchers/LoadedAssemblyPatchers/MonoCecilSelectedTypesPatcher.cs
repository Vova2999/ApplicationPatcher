using System;
using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	public class MonoCecilSelectedTypesPatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

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
				log.Info($"Patching type '{type.FullName}'...");

				log.Info($"Loading type '{type.FullName}'...");
				type.Load();
				log.Info($"Type '{type.FullName}' was loaded");

				type.MonoCecil.IsSealed = false;

				CreateEmptyInternalConstructor(assembly, type);
				SetVirtualOnProperties(type);
				SetVirtualOnMethods(type);

				log.Info($"Type '{type.FullName}' was patched");
			}

			foundedSelectedPatchingTypes.ForEach(type => type.Load().MonoCecil.IsSealed = false);
			log.Info("Selected types was patched");
			return PatchResult.Succeeded;
		}

		[AddLogOffset]
		private void CreateEmptyInternalConstructor(CommonAssembly assembly, CommonType type) {
			log.Info("Create constructor without parameters...");

			var emptyConstructor = type.GetConstructor();
			if (emptyConstructor != null) {
				emptyConstructor.MonoCecil.IsAssembly = true;
				log.Info("Constructor without parameters already created");
			}
			else {
				const MethodAttributes emptyConstructorMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
				var objectEmptyConstructorMethod = assembly.MonoCecil.MainModule.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes));

				var emptyConstructorMethod = new MethodDefinition(".ctor", emptyConstructorMethodAttributes, assembly.MonoCecil.MainModule.TypeSystem.Void);
				emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Call, objectEmptyConstructorMethod));
				emptyConstructorMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

				type.MonoCecil.Methods.Add(emptyConstructorMethod);
				log.Info("Constructor without parameters was created");
			}
		}

		[AddLogOffset]
		private void SetVirtualOnProperties(IHasProperties type) {
			log.Info("Patching properties...");

			if (!type.Properties.Any()) {
				log.Info("Not Found properties");
				return;
			}

			log.Debug("Properties found:", type.Properties.Select(property => property.FullName));
			foreach (var property in type.Properties) {
				SetVirtualMethod(property.MonoCecil.GetMethod);
				SetVirtualMethod(property.MonoCecil.SetMethod);
			}

			log.Info("Properties was patched");
		}

		[AddLogOffset]
		private void SetVirtualOnMethods(IHasMethods type) {
			log.Info("Patching non static methods...");

			var nonStaticMethods = type.Methods.Where(method => !method.MonoCecil.IsStatic).ToArray();
			if (!nonStaticMethods.Any()) {
				log.Info("Not found non static methods");
				return;
			}

			log.Debug("Non static methods found:", nonStaticMethods.Select(property => property.FullName));
			foreach (var method in nonStaticMethods)
				SetVirtualMethod(method.MonoCecil);

			log.Info("Non static methods was patched");
		}

		private static void SetVirtualMethod(MethodDefinition method) {
			if (method == null)
				return;

			method.IsFinal = false;

			if (method.IsVirtual)
				return;

			method.IsNewSlot = true;
			method.IsVirtual = true;
		}
	}
}