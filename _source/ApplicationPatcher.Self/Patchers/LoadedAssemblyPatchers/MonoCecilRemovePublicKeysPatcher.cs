using System.Linq;
using System.Runtime.CompilerServices;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;
using Mono.Cecil;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	[UsedImplicitly]
	public class MonoCecilRemovePublicKeysPatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly Log log;

		public MonoCecilRemovePublicKeysPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			RemoveFromMainAssembly(assembly);
			RemoveFromAssemblyReferences(assembly);
			RemoveFromInternalsVisibleToAttribute(assembly);
			return PatchResult.Succeeded;
		}

		[DoNotAddLogOffset]
		private void RemoveFromMainAssembly(CommonAssembly assembly) {
			log.Info("Clean assembly public key...");

			assembly.MainMonoCecilAssembly.Name.HasPublicKey = false;
			assembly.MainMonoCecilAssembly.Name.PublicKey = new byte[0];
			assembly.MainMonoCecilAssembly.Name.PublicKeyToken = new byte[0];
			assembly.MainMonoCecilAssembly.MainModule.Attributes &= ~ModuleAttributes.StrongNameSigned;

			log.Info("Assembly public key cleaned");
		}

		[DoNotAddLogOffset]
		private void RemoveFromAssemblyReferences(CommonAssembly assembly) {
			log.Info("Clean selected assembly references public key...");

			var assemblyReferences = assembly.MainMonoCecilAssembly.MainModule.AssemblyReferences
				.Where(assemblyReference => applicationPatcherSelfConfiguration.MonoCecilSelectedAssemblyReferenceNames.Contains(assemblyReference.Name))
				.ToArray();

			if (!assemblyReferences.Any()) {
				log.Info("Not found selected assembly references");
				return;
			}

			foreach (var assemblyReference in assemblyReferences) {
				log.Debug($"Clean public key from reference assembly '{assemblyReference.Name}'");

				assemblyReference.HasPublicKey = false;
				assemblyReference.PublicKey = new byte[0];
				assemblyReference.PublicKeyToken = new byte[0];
			}

			log.Info("Selected assembly references public key cleaned");
		}

		[DoNotAddLogOffset]
		private void RemoveFromInternalsVisibleToAttribute(CommonAssembly assembly) {
			log.Info("Clean public keys from selected InternalsVisibleToAttribute...");

			var selectedAttributes = assembly.GetAttributes<InternalsVisibleToAttribute>()
				.Select(commonAttribute => {
					var constructorArgument = commonAttribute.MonoCecilAttribute.ConstructorArguments.FirstOrDefault();
					var attributeParams = ((string)constructorArgument.Value).Split(',').Select(x => x.Trim()).ToList();
					var assemblyName = attributeParams.FirstOrDefault();
					return new { AssemblyName = assemblyName, AttributeParams = attributeParams, ConstructorArgument = constructorArgument, commonAttribute.MonoCecilAttribute };
				})
				.Where(attribute => applicationPatcherSelfConfiguration.MonoCecilSelectedInternalsVisibleToAttributeNames.Contains(attribute.AssemblyName))
				.ToArray();

			if (!selectedAttributes.Any()) {
				log.Info("Not found selected InternalsVisibleToAttribute");
				return;
			}

			foreach (var selectedAttribute in selectedAttributes) {
				selectedAttribute.AttributeParams.RemoveAll(attributeParam => attributeParam.StartsWith("PublicKey"));

				log.Debug($"Clean public key from InternalsVisibleToAttribute with assembly name '{selectedAttribute.AssemblyName}'");
				selectedAttribute.MonoCecilAttribute.ConstructorArguments.Clear();
				selectedAttribute.MonoCecilAttribute.ConstructorArguments.Add(
					new CustomAttributeArgument(selectedAttribute.ConstructorArgument.Type, string.Join(", ", selectedAttribute.AttributeParams)));
			}

			log.Info("Public keys from selected InternalsVisibleToAttribute cleaned");
		}
	}
}