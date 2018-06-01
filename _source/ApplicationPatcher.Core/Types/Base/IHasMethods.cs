using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasMethods {
		[UsedImplicitly]
		CommonMethod[] Methods { get; }
	}
}