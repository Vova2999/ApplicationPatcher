using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasTypesExtensions {
		public static IEnumerable<ICommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, Type baseType) {
			return hasTypes.Types.Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<ICommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, IHasType hasType) {
			return hasTypes.GetInheritanceCommonTypes(hasType.Type);
		}

		public static bool TryGetCommonType(this IHasTypes hasTypes, Type type, out ICommonType foundCommonType) {
			return (foundCommonType = hasTypes.GetCommonType(type)) != null;
		}
		public static bool TryGetCommonType(this IHasTypes hasTypes, string typeFullName, out ICommonType foundCommonType) {
			return (foundCommonType = hasTypes.GetCommonType(typeFullName)) != null;
		}

		public static ICommonType GetCommonType(this IHasTypes hasTypes, Type type, bool throwExceptionIfNotFound = false) {
			return (hasTypes.TypeTypeToTypes.TryGetValue(type, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).SingleOrDefault(throwExceptionIfNotFound, type.FullName);
		}
		public static ICommonType GetCommonType(this IHasTypes hasTypes, string typeFullName, bool throwExceptionIfNotFound = false) {
			return (hasTypes.TypeFullNameToTypes.TryGetValue(typeFullName, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).SingleOrDefault(throwExceptionIfNotFound, typeFullName);
		}
	}
}