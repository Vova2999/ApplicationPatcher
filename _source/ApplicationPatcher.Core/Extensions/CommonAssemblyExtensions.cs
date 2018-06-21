using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, Type type, bool throwExceptionIfNotFound = false) {
			return commonAssembly.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = false) {
			return commonAssembly.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}

		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.TypesFromThisAssembly.Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, CommonType baseCommonType) {
			return commonAssembly.GetInheritanceCommonTypesFromThisAssembly(baseCommonType.Reflection);
		}
	}
}