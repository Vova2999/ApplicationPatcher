using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasConstructorsExtensions {
		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, bool throwExceptionIfNotFound = false) {
			return hasConstructors.GetConstructor(Type.EmptyTypes, throwExceptionIfNotFound);
		}

		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Constructors.CheckLoaded().SingleOrDefault(constructor => constructor.ParameterTypes.SequenceEqual(constructorParameterTypes ?? Type.EmptyTypes), throwExceptionIfNotFound, ".ctor");
		}

		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, string[] constructorParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Constructors.CheckLoaded().SingleOrDefault(constructor => constructor.ParameterTypes.Select(type => type.FullName).SequenceEqual(constructorParameterTypeFullNames ?? new string[0]), throwExceptionIfNotFound, ".ctor");
		}
	}
}