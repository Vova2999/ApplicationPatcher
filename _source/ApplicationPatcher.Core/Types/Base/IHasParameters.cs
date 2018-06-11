using ApplicationPatcher.Core.Types.Common;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasParameters {
		CommonParameter[] Parameters { get; }
	}
}