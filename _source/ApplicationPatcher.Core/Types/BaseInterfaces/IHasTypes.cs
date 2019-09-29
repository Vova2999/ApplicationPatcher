using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasTypes {
		ICommonType[] Types { get; }

		IDictionary<Type, ICommonType[]> TypeTypeToTypes { get; }
		IDictionary<string, ICommonType[]> TypeFullNameToTypes { get; }
	}
}