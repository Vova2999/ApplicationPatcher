using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core {
	public interface IPatcher {
		PatchResult Patch(CommonAssembly assembly);
	}
}