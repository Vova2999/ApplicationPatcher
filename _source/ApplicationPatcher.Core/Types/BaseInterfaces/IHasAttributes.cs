using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Types.BaseInterfaces {
	[PublicAPI]
	public interface IHasAttributes {
		ICommonAttribute[] Attributes { get; }

		IDictionary<Type, ICommonAttribute[]> TypeTypeToAttributes { get; }
		IDictionary<string, ICommonAttribute[]> TypeFullNameToAttributes { get; }
	}
}