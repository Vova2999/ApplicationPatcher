using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasMethodsExtensions {
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasMethods.Methods.CheckLoaded().SingleOrDefault(method => method.Name == methodName && method.ParameterTypes.SequenceEqual(methodParameterTypes), throwExceptionIfNotFound, methodName);
		}
	}
}