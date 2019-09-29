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
			return (commonAssembly.TypeTypeToTypes.TryGetValue(type, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, type.FullName);
		}
		public static ICommonType GetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, IHasType hasType, bool throwExceptionIfNotFound = false) {
			return commonAssembly.GetCommonTypeFromThisAssembly(hasType.Type, throwExceptionIfNotFound);
		}
		public static ICommonType GetCommonTypeFromThisAssembly(this ICommonAssembly commonAssembly, string typeFullName, bool throwExceptionIfNotFound = false) {
			return (commonAssembly.TypeFullNameToTypes.TryGetValue(typeFullName, out var commonTypes) ? commonTypes : Enumerable.Empty<ICommonType>()).WhereFrom(commonAssembly).SingleOrDefault(throwExceptionIfNotFound, typeFullName);
		}

		public static IEnumerable<ICommonType> GetInheritanceCommonTypesFromThisAssembly(this ICommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.TypesFromThisAssembly.Where(type => type.IsInheritedFrom(baseType));
		}
		public static IEnumerable<ICommonType> GetInheritanceCommonTypesFromThisAssembly(this ICommonAssembly commonAssembly, IHasType baseHasType) {
			return commonAssembly.GetInheritanceCommonTypesFromThisAssembly(baseHasType.Type);
		}

		public static IEnumerable<TAttribute> GetReflectionAttributes<TAttribute>(this ICommonAssembly commonAssembly) where TAttribute : Attribute {
			return commonAssembly.Reflection.GetCustomAttributes(typeof(TAttribute), false).Select(attribute => (TAttribute)attribute);
		}
		public static bool TryGetReflectionAttribute<TAttribute>(this ICommonAssembly commonAssembly, out TAttribute foundAttribute) where TAttribute : Attribute {
			return (foundAttribute = commonAssembly.GetReflectionAttribute<TAttribute>()) != null;
		}
		public static TAttribute GetReflectionAttribute<TAttribute>(this ICommonAssembly commonAssembly, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)commonAssembly.GetReflectionAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static Attribute GetReflectionAttribute(this ICommonAssembly commonAssembly, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (Attribute)commonAssembly.Reflection.GetCustomAttributes(attributeType, false).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}

		public static bool ContainsReflectionAttribute<TAttribute>(this ICommonAssembly commonAssembly) where TAttribute : Attribute {
			return commonAssembly.ContainsReflectionAttribute(typeof(TAttribute));
		}
		public static bool ContainsReflectionAttribute(this ICommonAssembly commonAssembly, Type attributeType) {
			return commonAssembly.Reflection.GetCustomAttributes(attributeType, false).Any();
		}
		public static bool ContainsReflectionAttribute(this ICommonAssembly commonAssembly, IHasType attributeHasType) {
			return commonAssembly.ContainsReflectionAttribute(attributeHasType.Type);
		}

		public static bool NotContainsReflectionAttribute<TAttribute>(this ICommonAssembly commonAssembly) where TAttribute : Attribute {
			return !commonAssembly.ContainsReflectionAttribute<TAttribute>();
		}
		public static bool NotContainsReflectionAttribute(this ICommonAssembly commonAssembly, Type attributeType) {
			return !commonAssembly.ContainsReflectionAttribute(attributeType);
		}
		public static bool NotContainsReflectionAttribute(this ICommonAssembly commonAssembly, IHasType attributeHasType) {
			return !commonAssembly.ContainsReflectionAttribute(attributeHasType);
		}
	}
}