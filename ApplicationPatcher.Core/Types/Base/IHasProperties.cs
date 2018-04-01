using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasProperties {
		CommonProperty[] Properties { get; }
	}
}