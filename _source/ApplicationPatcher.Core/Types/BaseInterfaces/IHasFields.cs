using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasFields {
		ICommonField[] Fields { get; }

		IDictionary<string, ICommonField[]> FieldNameToFields { get; }
	}
}