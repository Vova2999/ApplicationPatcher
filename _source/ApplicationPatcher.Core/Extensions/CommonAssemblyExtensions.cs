using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class CommonAssemblyExtensions {
		public static bool TryGetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, Type type, out ICommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(type)) != null;
		}
		public static bool TryGetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, IHasType hasType, out ICommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(hasType)) != null;
		}
		public static bool TryGetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, string typeFullName, out ICommonType foundCommonType) {
			return (foundCommonType = commonAssembly.GetCommonTypeFromThisAssembly(typeFullName)) != null;
		}

		public static ICommonType GetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, Type type, bool throwExceptionIfNotFound = false) {
			return (commonAssembly.TypeTypeToTypes.CheckLoaded().TryGetValue(type, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, type.FullName);
		}
		public static ICommonType GetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, IHasType hasType, bool throwExceptionIfNotFound = false) {
			return commonAssembly.GetCommonTypeFromThisAssembly(hasType.Type, throwExceptionIfNotFound);
		}
		public static ICommonType GetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = false) {
			return (commonAssembly.TypeFullNameToTypes.CheckLoaded().TryGetValue(typeFullName, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, typeFullName);
		}

		public static IEnumerable<ICommonType> GetInheritanceCommonTypesFromThisAssembly(this ICommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.TypesFromThisAssembly.CheckLoaded().Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<ICommonType> GetInheritanceCommonTypesFromThisAssembly(this ICommonAssembly commonAssembly, IHasType baseHasType) {
			return commonAssembly.GetInheritanceCommonTypesFromThisAssembly(baseHasType.Type);
		}
	}
}