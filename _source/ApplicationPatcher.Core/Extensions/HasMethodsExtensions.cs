using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasMethodsExtensions {
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, out ICommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, out ICommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterTypes)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, IHasType[] methodParameterHasTypes, out ICommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterHasTypes)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, out ICommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterTypeFullNames)) != null;
		}

		public static ICommonMethod GetMethod(this IHasMethods hasMethods, string methodName, bool throwExceptionIfNotFound = false) {
			return hasMethods.GetMethod(methodName, Type.EmptyTypes, throwExceptionIfNotFound);
		}
		public static ICommonMethod GetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, bool throwExceptionIfNotFound = false) {
			return (hasMethods.MethodNameToMethods.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<ICommonMethod>()).Where(method => method.ParameterTypes.SequenceEqual(methodParameterTypes ?? Type.EmptyTypes)).SingleOrDefault(throwExceptionIfNotFound, methodName);
		}
		public static ICommonMethod GetMethod(this IHasMethods hasMethods, string methodName, IHasType[] methodParameterHasTypes, bool throwExceptionIfNotFound = false) {
			return hasMethods.GetMethod(methodName, methodParameterHasTypes.Select(type => type.Type).ToArray(), throwExceptionIfNotFound);
		}
		public static ICommonMethod GetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return (hasMethods.MethodNameToMethods.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<ICommonMethod>()).Where(method => method.ParameterTypes.Select(type => type.FullName.NullIfEmpty() ?? type.Name).SequenceEqual(methodParameterTypeFullNames ?? new string[0])).SingleOrDefault(throwExceptionIfNotFound, methodName);
		}

		public static IEnumerable<ICommonMethod> GetMethods(this IHasMethods hasMethods, string methodName) {
			return hasMethods.MethodNameToMethods.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<ICommonMethod>();
		}
	}
}