using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Core {
	internal interface IPatcher {
		PatchResult Patch(ICommonAssembly assembly);
	}
}