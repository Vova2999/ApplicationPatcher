using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Self.Patchers.OnNotLoadedAssembly {
	public class CheckAssemblyPublicKeyPatcher : PatcherOnNotLoadedApplication {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;

		public CheckAssemblyPublicKeyPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
		}

		public override PatchResult Patch(ICommonAssembly assembly) {
			return assembly.MonoCecil.Name.PublicKeyToken.SequenceEqual(applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken ?? new byte[0])
				? PatchResult.Cancel
				: PatchResult.Continue;
		}
	}
}