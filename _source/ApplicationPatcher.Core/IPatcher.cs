using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core {
	public interface IPatcher {
		void Patch(CommonAssembly assembly);
	}
}