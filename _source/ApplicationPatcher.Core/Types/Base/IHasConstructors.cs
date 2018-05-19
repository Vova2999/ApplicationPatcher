using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasConstructors {
		CommonConstructor[] Constructors { get; }
	}
}