using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasProperties {
		ICommonProperty[] Properties { get; }

		IDictionary<string, ICommonProperty[]> PropertyNameToProperties { get; }
	}
}