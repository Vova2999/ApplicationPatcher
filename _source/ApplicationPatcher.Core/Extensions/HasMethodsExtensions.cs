using System;
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
			return hasMethods.Load().Methods.SingleOrDefault(method => method.Name == methodName && method.ParameterTypes.SequenceEqual(methodParameterTypes ?? Type.EmptyTypes), throwExceptionIfNotFound, methodName);
		}
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, IHasType[] methodParameterHasTypes, bool throwExceptionIfNotFound = false) {
			return hasMethods.GetMethod(methodName, methodParameterHasTypes.Select(type => type.Type).ToArray(), throwExceptionIfNotFound);
		}
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return hasMethods.Load().Methods.SingleOrDefault(method => method.Name == methodName && method.ParameterTypes.Select(type => type.FullName.NullIfEmpty() ?? type.Name).SequenceEqual(methodParameterTypeFullNames ?? new string[0]), throwExceptionIfNotFound, ".ctor");
		}

		public static CommonMethod[] GetMethods(this IHasMethods hasMethods, string methodName) {
			return hasMethods.Load().Methods.Where(method => method.Name == methodName).ToArray();
		}
	}
}