using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core {
	internal interface IPatcher {
		PatchResult Patch(CommonAssembly assembly);
	}
}