using System;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.Base {
	public interface IHasType {
		[UsedImplicitly]
		Type Type { get; }
	}
}