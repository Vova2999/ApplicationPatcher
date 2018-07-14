using System;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasConstructorsExtensions {
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, out CommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor()) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, out CommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterTypes)) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, IHasType[] constructorParameterHasTypes, out CommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterHasTypes)) != null;
		}
		public static bool TryGetConstructor(this IHasConstructors hasConstructors, string[] constructorParameterTypeFullNames, out CommonConstructor foundCommonConstructor) {
			return (foundCommonConstructor = hasConstructors.GetConstructor(constructorParameterTypeFullNames)) != null;
		}

		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, bool throwExceptionIfNotFound = false) {
			return hasConstructors.GetConstructor(Type.EmptyTypes, throwExceptionIfNotFound);
		}
		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Load().Constructors.SingleOrDefault(constructor => constructor.ParameterTypes.SequenceEqual(constructorParameterTypes ?? Type.EmptyTypes), throwExceptionIfNotFound, ".ctor");
		}
		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, IHasType[] constructorParameterHasTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.GetConstructor(constructorParameterHasTypes.Select(type => type.Type).ToArray(), throwExceptionIfNotFound);
		}
		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, string[] constructorParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Load().Constructors.SingleOrDefault(constructor => constructor.ParameterTypes.Select(type => type.FullName.NullIfEmpty() ?? type.Name).SequenceEqual(constructorParameterTypeFullNames ?? new string[0]), throwExceptionIfNotFound, ".ctor");
		}
	}
}