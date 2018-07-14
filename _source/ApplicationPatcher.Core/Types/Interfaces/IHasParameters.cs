using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasParameters : ICommonMember<IHasParameters> {
		CommonParameter[] Parameters { get; }
	}
}