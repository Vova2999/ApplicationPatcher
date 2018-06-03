using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Self.Patchers.NotLoadedAssemblyPatchers {
	public class CheckAssemblyPublicKeyPatcher : NotLoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;

		public CheckAssemblyPublicKeyPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			return assembly.MonoCecil.Name.PublicKeyToken.SequenceEqual(applicationPatcherSelfConfiguration.MonoCecilNewPublicKeyToken ?? new byte[0])
				? PatchResult.Canceled
				: PatchResult.Succeeded;
		}
	}
}