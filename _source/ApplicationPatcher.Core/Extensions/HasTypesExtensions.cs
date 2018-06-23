using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasTypesExtensions {
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, Type baseType) {
			return hasTypes.Types.CheckLoaded().Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, IHasType hasType) {
			return hasTypes.GetInheritanceCommonTypes(hasType.Type);
		}

		public static bool TryGetCommonType(this IHasTypes hasTypes, Type type, out CommonType foundCommonType) {
			return (foundCommonType = hasTypes.GetCommonType(type)) != null;
		}
		public static bool TryGetCommonType(this IHasTypes hasTypes, string typeFullName, out CommonType foundCommonType) {
			return (foundCommonType = hasTypes.GetCommonType(typeFullName)) != null;
		}

		public static CommonType GetCommonType(this IHasTypes hasTypes, Type type, bool throwExceptionIfNotFound = false) {
			return hasTypes.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonType(this IHasTypes hasTypes, string typeFullName, bool throwExceptionIfNotFound = false) {
			return hasTypes.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}
	}
}