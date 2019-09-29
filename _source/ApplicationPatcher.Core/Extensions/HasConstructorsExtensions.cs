using System;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasConstructorsExtensions {
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, out ICommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor()) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, out ICommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterTypes)) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, IHasType[] constructorParameterHasTypes, out ICommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterHasTypes)) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, string[] constructorParameterTypeFullNames, out ICommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterTypeFullNames)) != null;
		}

		public static ICommonConstructor GetConstructor(this IHasConstructors hasConstructors, bool throwExceptionIfNotFound = false) {
			return hasConstructors.GetConstructor(Type.EmptyTypes, throwExceptionIfNotFound);
		}
		public static ICommonConstructor GetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Constructors.CheckLoaded().SingleOrDefault(constructor => constructor.ParameterTypes.SequenceEqual(constructorParameterTypes ?? Type.EmptyTypes), throwExceptionIfNotFound, ".ctor");
		}
		public static ICommonConstructor GetConstructor(this IHasConstructors hasConstructors, IHasType[] constructorParameterHasTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.GetConstructor(constructorParameterHasTypes.Select(type => type.Type).ToArray(), throwExceptionIfNotFound);
		}
		public static ICommonConstructor GetConstructor(this IHasConstructors hasConstructors, string[] constructorParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Constructors.CheckLoaded().SingleOrDefault(constructor => constructor.ParameterTypes.Select(type => type.FullName.NullIfEmpty() ?? type.Name).SequenceEqual(constructorParameterTypeFullNames ?? new string[0]), throwExceptionIfNotFound, ".ctor");
		}
	}
}