using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class LoadedAssemblyPatcher : IPatcher {
		[AddLogOffset]
		public abstract PatchResult Patch(CommonAssembly assembly);
	}
}