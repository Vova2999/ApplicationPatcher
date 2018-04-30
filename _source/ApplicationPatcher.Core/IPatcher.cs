using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core {
	internal interface IPatcher {
		PatchResult Patch(CommonAssembly assembly);
	}
}