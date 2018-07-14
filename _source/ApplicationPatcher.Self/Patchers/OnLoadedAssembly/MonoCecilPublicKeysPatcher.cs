using System.Linq;
using System.Runtime.CompilerServices;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.CommonMembers;
using Mono.Cecil;

namespace ApplicationPatcher.Self.Patchers.OnLoadedAssembly {
	public class MonoCecilPublicKeysPatcher : PatcherOnLoadedApplication {
		private const string moqAssemblyName = "DynamicProxyGenAssembly2";
		private const string moqAssemblyPublicKey = "0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";

		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

		public MonoCecilPublicKeysPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			RewriteMainAssemblyPublicKey(assembly);
			RewriteSelectedAssemblyReferencesPublicKey(assembly);
			RewriteOrCreateInternalVisibleToAttributes(assembly);

			return PatchResult.Continue;
		}

		private void RewriteMainAssemblyPublicKey(CommonAssembly assembly) {
			log.Info("Rewrite assembly public key...");

			assembly.MonoCecil.Name.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey;
			assembly.MonoCecil.Name.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken;

			log.Info("Assembly public key was rewrited");
		}

		private void RewriteSelectedAssemblyReferencesPublicKey(CommonAssembly assembly) {
			log.Info("Rewrite selected assembly references public key...");

			var assemblyReferences = assembly.MonoCecil.MainModule.AssemblyReferences
				.Where(assemblyReference => applicationPatcherSelfConfiguration.MonoCecilSelectedAssemblyReferenceNames.Contains(assemblyReference.Name))
				.ToArray();

			if (!assemblyReferences.Any()) {
				log.Info("Not found selected assembly references");
				return;
			}

			foreach (var assemblyReference in assemblyReferences) {
				log.Debug($"Rewrite public key from reference assembly '{assemblyReference.Name}'");

				assemblyReference.PublicKey = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey;
				assemblyReference.PublicKeyToken = applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken;
			}

			log.Info("Selected assembly references public key was rewrited");
		}

		private void RewriteOrCreateInternalVisibleToAttributes(CommonAssembly assembly) {
			log.Info($"Create InternalsVisibleToAttribute for '{moqAssemblyName}'...");

			var internalsVisibleToAttributes = assembly.GetAttributes<InternalsVisibleToAttribute>()
				.Select(commonAttribute => {
					var constructorArgument = commonAttribute.MonoCecil.ConstructorArguments.FirstOrDefault();
					var attributeParams = ((string)constructorArgument.Value).Split(',').Select(x => x.Trim()).ToList();
					var assemblyName = attributeParams.FirstOrDefault();
					return new { AssemblyName = assemblyName, AttributeParams = attributeParams, ConstructorArgument = constructorArgument, commonAttribute.MonoCecil };
				})
				.ToArray();

			if (internalsVisibleToAttributes.Any(attribute => attribute.AssemblyName == moqAssemblyName)) {
				log.Info($"InternalsVisibleToAttribute for '{moqAssemblyName}' already created");
			}
			else {
				var internalsVisibleToAttribute = new CustomAttribute(assembly.MonoCecil.MainModule.ImportReference(typeof(InternalsVisibleToAttribute).GetConstructor(new[] { typeof(string) })));
				internalsVisibleToAttribute.ConstructorArguments.Add(new CustomAttributeArgument(assembly.MonoCecil.MainModule.TypeSystem.String, $"{moqAssemblyName}, PublicKey={moqAssemblyPublicKey}"));
				assembly.MonoCecil.CustomAttributes.Add(internalsVisibleToAttribute);

				log.Info($"InternalsVisibleToAttribute for '{moqAssemblyName}' was created");
			}

			log.Info("Rewrite public keys from selected InternalsVisibleToAttributes...");

			var selectedInternalsVisibleToAttributes = internalsVisibleToAttributes
				.Where(attribute => applicationPatcherSelfConfiguration.MonoCecilSelectedInternalsVisibleToAttributeNames.Contains(attribute.AssemblyName))
				.ToArray();

			if (!selectedInternalsVisibleToAttributes.Any()) {
				log.Info("Not found selected InternalsVisibleToAttributes");
				return;
			}

			foreach (var selectedInternalsVisibleToAttribute in selectedInternalsVisibleToAttributes) {
				var attributeParams = selectedInternalsVisibleToAttribute.AttributeParams
					.Where(attributeParam => !attributeParam.StartsWith("PublicKey"))
					.Concat(new[] { $"PublicKey={applicationPatcherSelfConfiguration.MonoCecilNewPublicKey.ToHexString()}" })
					.ToArray();

				log.Debug($"Rewrite public key from InternalsVisibleToAttribute with assembly name '{selectedInternalsVisibleToAttribute.AssemblyName}'");
				selectedInternalsVisibleToAttribute.MonoCecil.ConstructorArguments.Clear();
				selectedInternalsVisibleToAttribute.MonoCecil.ConstructorArguments.Add(
					new CustomAttributeArgument(selectedInternalsVisibleToAttribute.ConstructorArgument.Type, attributeParams.JoinToString(", ")));
			}

			log.Info("Public keys from selected InternalsVisibleToAttributes was rewrited");
		}
	}
}