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
	public class MonoCecilPublicKeysPatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly Log log;

		public MonoCecilPublicKeysPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			MainAssembly(assembly);
			AssemblyReferences(assembly);
			InternalVisibleToAttributes(assembly);

			return PatchResult.Succeeded;
		}

		[DoNotAddLogOffset]
		private void MainAssembly(CommonAssembly assembly) {
			log.Info("Rewrite assembly public key...");

			assembly.MainMonoCecilAssembly.Name.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey;
			//assembly.MainMonoCecilAssembly.Name.PublicKey = new byte[0];
			assembly.MainMonoCecilAssembly.Name.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken;

			log.Info("Assembly public key rewrited");
		}

		[DoNotAddLogOffset]
		private void AssemblyReferences(CommonAssembly assembly) {
			log.Info("Rewrite selected assembly references public key...");

			var assemblyReferences = assembly.MainMonoCecilAssembly.MainModule.AssemblyReferences
				.Where(assemblyReference => applicationPatcherSelfConfiguration.MonoCecilSelectedAssemblyReferenceNames.Contains(assemblyReference.Name))
				.ToArray();

			if (!assemblyReferences.Any()) {
				log.Info("Not found selected assembly references");
				return;
			}

			foreach (var assemblyReference in assemblyReferences) {
				log.Debug($"Rewrite public key from reference assembly '{assemblyReference.Name}'");

				assemblyReference.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey;
				//assemblyReference.PublicKey = new byte[0];
				assemblyReference.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken;
			}

			log.Info("Selected assembly references public key rewrited");
		}

		[DoNotAddLogOffset]
		private void InternalVisibleToAttributes(CommonAssembly assembly) {
			log.Info("Rewrite public keys from selected InternalsVisibleToAttribute...");

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
				var attributeParams = selectedAttribute.AttributeParams
					.Where(attributeParam => !attributeParam.StartsWith("PublicKey"))
					.Concat(new[] { $"PublicKey={applicationPatcherSelfConfiguration.MonoCecilNewPublicKey.ToHexString()}" })
					.ToArray();

				log.Debug($"Rewrite public key from InternalsVisibleToAttribute with assembly name '{selectedAttribute.AssemblyName}'");
				selectedAttribute.MonoCecilAttribute.ConstructorArguments.Clear();
				selectedAttribute.MonoCecilAttribute.ConstructorArguments.Add(
					new CustomAttributeArgument(selectedAttribute.ConstructorArgument.Type, string.Join(", ", attributeParams)));
			}

			log.Info("Public keys from selected InternalsVisibleToAttribute rewrited");
		}
	}
}