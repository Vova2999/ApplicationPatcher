using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasParameters {
		CommonParameter[] Parameters { get; }
	}
}