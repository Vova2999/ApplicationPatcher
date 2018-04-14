using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self.Patchers.NotLoadedAssemblyPatchers {
	[UsedImplicitly]
	public class CheckAssemblyPublicKey : INotLoadedAssemblyPatcher {
		public PatchResult Patch(CommonAssembly assembly) {
			return assembly.MainMonoCecilAssembly.Name.HasPublicKey ? PatchResult.Succeeded : PatchResult.Canceled;
		}
	}
}