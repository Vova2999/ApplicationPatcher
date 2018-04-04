using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasFields {
		[UsedImplicitly]
		CommonField[] Fields { get; }
	}
}