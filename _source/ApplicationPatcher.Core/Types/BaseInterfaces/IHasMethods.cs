using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasMethods {
		ICommonMethod[] Methods { get; }

		IDictionary<string, ICommonMethod[]> MethodNameToMethods { get; }
	}
}