using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasFields {
		CommonField[] Fields { get; }
	}
}