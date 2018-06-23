using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, Type type, bool throwExceptionIfNotFound = false) {
			return commonAssembly.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, IHasType hasType, bool throwExceptionIfNotFound = false) {
			return commonAssembly.GetCommonTypeFromThisAssembly(hasType.Type, throwExceptionIfNotFound);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = false) {
			return commonAssembly.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}

		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.TypesFromThisAssembly.Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, IHasType baseHasType) {
			return commonAssembly.GetInheritanceCommonTypesFromThisAssembly(baseHasType.Type);
		}
	}
}