using System;
using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ApplicationPatcher.Self.Patchers.OnLoadedAssembly {
	public class MonoCecilSelectedTypesPatcher : PatcherOnLoadedApplication {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

		public MonoCecilSelectedTypesPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(ICommonAssembly assembly) {
			log.Info("Patching selected types...");
			var foundSelectedPatchingTypes = applicationPatcherSelfConfiguration.MonoCecilSelectedPatchingTypeFullNames
				.Select(typeFullName => assembly.GetCommonTypeFromThisAssembly(typeFullName))
				.Where(type => type != null)
				.ToArray();

			if (!foundSelectedPatchingTypes.Any()) {
				log.Info("Not found selected types");
				return PatchResult.Continue;
			}

			log.Debug("Selected types found:", foundSelectedPatchingTypes.Select(viewModel => viewModel.FullName).OrderBy(fullName => fullName));

			foreach (var type in foundSelectedPatchingTypes) {
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

			foundSelectedPatchingTypes.ForEach(type => type.MonoCecil.IsSealed = false);
			log.Info("Selected types was patched");
			return PatchResult.Continue;
		}

		[AddLogOffset]
		private void CreateEmptyInternalConstructor(ICommonAssembly assembly, ICommonType type) {
			log.Info("Create constructor without parameters...");

			var emptyConstructor = type.GetConstructor();
			if (emptyConstructor != null) {
				emptyConstructor.MonoCecil.IsAssembly = true;
				log.Info("Constructor without parameters already created");
			}
			else {
				const MethodAttributes emptyConstructorMethodAttributes = MethodAttributes.Assembly | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
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