using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace ApplicationPatcher.Core.Types.Interfaces {
	public interface IHasType : ICommonMember<IHasType> {
		Type Type { get; }
	}
}