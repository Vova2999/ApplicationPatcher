using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasTypeExtensions {
		public static bool Is(this IHasType hasType, Type type) {
			return hasType.Type == type;
		}
		public static bool Is(this IHasType hasType, string typeFullName) {
			return hasType.Type.FullName == typeFullName;
		}
		public static bool Is(this IHasType hasType, IHasType thatHasType) {
			return hasType.Is(thatHasType.Type);
		}

		public static bool IsNot(this IHasType hasType, Type type) {
			return !hasType.Is(type);
		}
		public static bool IsNot(this IHasType hasType, string typeFullName) {
			return !hasType.Is(typeFullName);
		}
		public static bool IsNot(this IHasType hasType, IHasType thatHasType) {
			return !hasType.Is(thatHasType);
		}

		public static bool IsInheritedFrom(this IHasType hasType, Type type) {
			return type.IsAssignableFrom(hasType.Type);
		}
		public static bool IsInheritedFrom(this IHasType hasType, IHasType thatHasType) {
			return hasType.IsInheritedFrom(thatHasType.Type);
		}

		public static bool IsNotInheritedFrom(this IHasType hasType, Type type) {
			return !hasType.IsInheritedFrom(type);
		}
		public static bool IsNotInheritedFrom(this IHasType hasType, IHasType thatHasType) {
			return !hasType.IsInheritedFrom(thatHasType);
		}

		public static IEnumerable<TAttribute> GetReflectionAttributes<TAttribute>(this IHasType hasType) where TAttribute : Attribute {
			return hasType.Type.GetCustomAttributes<TAttribute>(false);
		}
		public static bool TryGetReflectionAttribute<TAttribute>(this IHasType hasType, out TAttribute foundAttribute) where TAttribute : Attribute {
			return (foundAttribute = hasType.GetReflectionAttribute<TAttribute>()) != null;
		}
		public static TAttribute GetReflectionAttribute<TAttribute>(this IHasType hasType, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)hasType.GetReflectionAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static Attribute GetReflectionAttribute(this IHasType hasType, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (Attribute)hasType.Type.GetCustomAttributes(attributeType, false).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}

		public static bool ContainsReflectionAttribute<TAttribute>(this IHasType hasType) where TAttribute : Attribute {
			return hasType.ContainsReflectionAttribute(typeof(TAttribute));
		}
		public static bool ContainsReflectionAttribute(this IHasType hasType, Type attributeType) {
			return hasType.Type.GetCustomAttributes(attributeType, false).Any();
		}
		public static bool ContainsReflectionAttribute(this IHasType hasType, IHasType attributeHasType) {
			return hasType.ContainsReflectionAttribute(attributeHasType.Type);
		}

		public static bool NotContainsReflectionAttribute<TAttribute>(this IHasType hasType) where TAttribute : Attribute {
			return !hasType.ContainsReflectionAttribute<TAttribute>();
		}
		public static bool NotContainsReflectionAttribute(this IHasType hasType, Type attributeType) {
			return !hasType.ContainsReflectionAttribute(attributeType);
		}
		public static bool NotContainsReflectionAttribute(this IHasType hasType, IHasType attributeHasType) {
			return !hasType.ContainsReflectionAttribute(attributeHasType);
		}
	}
}