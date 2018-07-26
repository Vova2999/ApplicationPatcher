using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasMethodsExtensions {
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, out CommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, out CommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterTypes)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, IHasType[] methodParameterHasTypes, out CommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterHasTypes)) != null;
		}
		public static bool TryGetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, out CommonMethod foundCommonMethod) {
			return (foundCommonMethod = hasMethods.GetMethod(methodName, methodParameterTypeFullNames)) != null;
		}

		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, bool throwExceptionIfNotFound = false) {
			return hasMethods.GetMethod(methodName, Type.EmptyTypes, throwExceptionIfNotFound);
		}
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, bool throwExceptionIfNotFound = false) {
			return (hasMethods.Load().MethodNameToMethod.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<CommonMethod>()).Where(method => method.ParameterTypes.SequenceEqual(methodParameterTypes ?? Type.EmptyTypes)).SingleOrDefault(throwExceptionIfNotFound, methodName);
		}
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, IHasType[] methodParameterHasTypes, bool throwExceptionIfNotFound = false) {
			return hasMethods.GetMethod(methodName, methodParameterHasTypes.Select(type => type.Type).ToArray(), throwExceptionIfNotFound);
		}
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return (hasMethods.Load().MethodNameToMethod.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<CommonMethod>()).Where(method => method.ParameterTypes.Select(type => type.FullName.NullIfEmpty() ?? type.Name).SequenceEqual(methodParameterTypeFullNames ?? new string[0])).SingleOrDefault(throwExceptionIfNotFound, methodName);
		}

		public static IEnumerable<CommonMethod> GetMethods(this IHasMethods hasMethods, string methodName) {
			return hasMethods.Load().MethodNameToMethod.TryGetValue(methodName, out var commonMethods) ? commonMethods : Enumerable.Empty<CommonMethod>();
		}
	}
}