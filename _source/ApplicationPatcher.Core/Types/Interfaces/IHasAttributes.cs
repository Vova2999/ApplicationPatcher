using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasAttributes : ICommonMember<IHasAttributes> {
		CommonAttribute[] Attributes { get; }
	}
}