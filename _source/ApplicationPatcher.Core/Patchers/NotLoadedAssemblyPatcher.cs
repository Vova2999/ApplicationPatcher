using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class NotLoadedAssemblyPatcher : IPatcher {
		[AddLogOffset]
		public abstract PatchResult Patch(CommonAssembly assembly);
	}
}