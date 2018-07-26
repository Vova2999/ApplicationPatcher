using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasMethods : ICommonMember<IHasMethods> {
		CommonMethod[] Methods { get; }

		Dictionary<string, CommonMethod[]> MethodNameToMethod { get; }
	}
}