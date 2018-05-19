using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasAttributes {
		CommonAttribute[] Attributes { get; }
	}
}