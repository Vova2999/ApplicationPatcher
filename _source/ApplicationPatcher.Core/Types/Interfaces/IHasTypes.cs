using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasTypes : ICommonMember<IHasTypes> {
		CommonType[] Types { get; }

		Dictionary<Type, CommonType[]> TypeTypeToType { get; }
		Dictionary<string, CommonType[]> TypeFullNameToType { get; }
	}
}