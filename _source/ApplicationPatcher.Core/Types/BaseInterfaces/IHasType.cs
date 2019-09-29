using System;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasType {
		// todo: mb CommonType?
		Type Type { get; }
	}
}