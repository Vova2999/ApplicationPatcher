using ApplicationPatcher.Core.Types.Common;

// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core {
	internal interface IPatcher {
		PatchResult Patch(CommonAssembly assembly);
	}
}