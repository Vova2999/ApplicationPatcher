using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasAttributes : ICommonMember<IHasAttributes> {
		CommonAttribute[] Attributes { get; }

		Dictionary<Type, CommonAttribute[]> TypeTypeToAttribute { get; }
		Dictionary<string, CommonAttribute[]> TypeFullNameToAttribute { get; }
	}
}