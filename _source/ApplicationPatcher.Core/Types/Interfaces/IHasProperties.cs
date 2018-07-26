using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasProperties : ICommonMember<IHasProperties> {
		CommonProperty[] Properties { get; }

		Dictionary<string, CommonProperty[]> PropertyNameToProperty { get; }
	}
}