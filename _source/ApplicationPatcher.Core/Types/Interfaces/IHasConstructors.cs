﻿using ApplicationPatcher.Core.Types.CommonMembers;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasConstructors : ICommonMember<IHasConstructors> {
		CommonConstructor[] Constructors { get; }
	}
}