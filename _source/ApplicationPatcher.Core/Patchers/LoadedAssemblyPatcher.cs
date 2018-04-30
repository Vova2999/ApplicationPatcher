using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class LoadedAssemblyPatcher : IPatcher {
		public abstract PatchResult Patch(CommonAssembly assembly);
	}
}