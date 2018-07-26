using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasAttributesExtensions {
		public static IEnumerable<TAttribute> GetReflectionAttributes<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.GetAttributes<TAttribute>().Select(attribute => (TAttribute)attribute.Reflection);
		}
		public static bool TryGetReflectionAttribute<TAttribute>(this IHasAttributes hasAttributes, out TAttribute foundAttribute) where TAttribute : Attribute {
			return (foundAttribute = hasAttributes.GetReflectionAttribute<TAttribute>()) != null;
		}
		public static TAttribute GetReflectionAttribute<TAttribute>(this IHasAttributes hasAttributes, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)hasAttributes.GetAttribute<TAttribute>(throwExceptionIfNotFound)?.Reflection;
		}

		public static bool TryGetAttribute<TAttribute>(this IHasAttributes hasAttributes, out CommonAttribute foundCommonAttribute) where TAttribute : Attribute {
			return (foundCommonAttribute = hasAttributes.GetAttribute<TAttribute>()) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, Type attributeType, out CommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeType)) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType, out CommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeHasType)) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName, out CommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeTypeFullName)) != null;
		}

		public static CommonAttribute GetAttribute<TAttribute>(this IHasAttributes hasAttributes, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return hasAttributes.GetAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (hasAttributes.Load().TypeTypeToAttribute.TryGetValue(attributeType, out var commonAttributes) ? commonAttributes : Enumerable.Empty<CommonAttribute>()).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType, bool throwExceptionIfNotFound = false) {
			return hasAttributes.GetAttribute(attributeHasType.Type, throwExceptionIfNotFound);
		}
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName, bool throwExceptionIfNotFound = false) {
			return (hasAttributes.Load().TypeFullNameToAttribute.TryGetValue(attributeTypeFullName, out var commonAttributes) ? commonAttributes : Enumerable.Empty<CommonAttribute>()).SingleOrDefault(throwExceptionIfNotFound, attributeTypeFullName);
		}

		public static IEnumerable<CommonAttribute> GetAttributes<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.GetAttributes(typeof(TAttribute));
		}
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Load().TypeTypeToAttribute.TryGetValue(attributeType, out var commonAttributes) ? commonAttributes : Enumerable.Empty<CommonAttribute>();
		}
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, IHasType attributeHasType) {
			return hasAttributes.GetAttributes(attributeHasType.Type);
		}
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Load().TypeFullNameToAttribute.TryGetValue(attributeTypeFullName, out var commonAttributes) ? commonAttributes : Enumerable.Empty<CommonAttribute>();
		}

		public static bool ContainsAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.ContainsAttribute(typeof(TAttribute));
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Load().TypeTypeToAttribute.ContainsKey(attributeType);
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType) {
			return hasAttributes.ContainsAttribute(attributeHasType.Type);
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Load().TypeFullNameToAttribute.ContainsKey(attributeTypeFullName);
		}

		public static bool NotContainsAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return !hasAttributes.ContainsAttribute<TAttribute>();
		}
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return !hasAttributes.ContainsAttribute(attributeType);
		}
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType) {
			return !hasAttributes.ContainsAttribute(attributeHasType);
		}
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return !hasAttributes.ContainsAttribute(attributeTypeFullName);
		}
	}
}