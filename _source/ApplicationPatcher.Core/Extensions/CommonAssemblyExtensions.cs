using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		public static bool TryGetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, Type type, out CommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(type)) != null;
		}
		public static bool TryGetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, IHasType hasType, out CommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(hasType)) != null;
		}
		public static bool TryGetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, string typeFullName, out CommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(typeFullName)) != null;
		}

		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, Type type, bool throwExceptionIfNotFound = false) {
			return (((IHasTypes)commonAssembly.Load()).TypeTypeToType.TryGetValue(type, out var commonTypes) ? commonTypes : Enumerable.Empty<CommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, IHasType hasType, bool throwExceptionIfNotFound = false) {
			return commonAssembly.GetCommonTypeFromThisAssembly(hasType.Type, throwExceptionIfNotFound);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = false) {
			return (((IHasTypes)commonAssembly.Load()).TypeFullNameToType.TryGetValue(typeFullName, out var commonTypes) ? commonTypes : Enumerable.Empty<CommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, typeFullName);
		}

		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.TypesFromThisAssembly.Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<CommonType> GetInheritanceCommonTypesFromThisAssembly(this CommonAssembly commonAssembly, IHasType baseHasType) {
			return commonAssembly.GetInheritanceCommonTypesFromThisAssembly(baseHasType.Type);
		}
	}
}