using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class NotLoadedAssemblyPatcher : IPatcher {
		public abstract PatchResult Patch(CommonAssembly assembly);
	}
}