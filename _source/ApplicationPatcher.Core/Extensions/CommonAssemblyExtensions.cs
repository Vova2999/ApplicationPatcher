using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable MemberCanBePrivate.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this CommonAssembly commonAssembly, CommonType baseCommonType) {
			return commonAssembly.GetInheritanceCommonTypes(baseCommonType.ReflectionType);
		}
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this CommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.Types.CheckLoaded().Where(type => type.IsInheritedFrom(baseType));
		}

		public static CommonType GetCommonType(this CommonAssembly commonAssembly, Type type, bool throwExceptionIfNotFound = true) {
			return commonAssembly.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonType(this CommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = true) {
			return commonAssembly.Types.CheckLoaded().SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}
	}
}