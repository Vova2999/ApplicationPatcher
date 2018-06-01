using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasTypesExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, Type baseType) {
			return hasTypes.Types.CheckLoaded().Where(type => type.IsInheritedFrom(baseType));
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this IHasTypes hasTypes, CommonType baseCommonType) {
			return hasTypes.GetInheritanceCommonTypes(baseCommonType.Reflection);
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonType GetCommonType(this IHasTypes hasTypes, Type type, bool throwExceptionIfNotFound = false) {
			return hasTypes.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonType GetCommonType(this IHasTypes hasTypes, string typeFullName, bool throwExceptionIfNotFound = false) {
			return hasTypes.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}
	}
}