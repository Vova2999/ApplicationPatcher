using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasAttributesExtensions {
		public static bool TryGetAttribute<TAttribute>(this IHasAttributes hasAttributes, out ICommonAttribute foundCommonAttribute) where TAttribute : Attribute {
			return (foundCommonAttribute = hasAttributes.GetAttribute<TAttribute>()) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, Type attributeType, out ICommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeType)) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType, out ICommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeHasType)) != null;
		}
		public static bool TryGetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName, out ICommonAttribute foundCommonAttribute) {
			return (foundCommonAttribute = hasAttributes.GetAttribute(attributeTypeFullName)) != null;
		}

		public static ICommonAttribute GetAttribute<TAttribute>(this IHasAttributes hasAttributes, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return hasAttributes.GetAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static ICommonAttribute GetAttribute(this IHasAttributes hasAttributes, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (hasAttributes.TypeTypeToAttributes.TryGetValue(attributeType, out var commonAttributes) ? commonAttributes : Enumerable.Empty<ICommonAttribute>()).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}
		public static ICommonAttribute GetAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType, bool throwExceptionIfNotFound = false) {
			return hasAttributes.GetAttribute(attributeHasType.Type, throwExceptionIfNotFound);
		}
		public static ICommonAttribute GetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName, bool throwExceptionIfNotFound = false) {
			return (hasAttributes.TypeFullNameToAttributes.TryGetValue(attributeTypeFullName, out var commonAttributes) ? commonAttributes : Enumerable.Empty<ICommonAttribute>()).SingleOrDefault(throwExceptionIfNotFound, attributeTypeFullName);
		}

		public static IEnumerable<ICommonAttribute> GetAttributes<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.GetAttributes(typeof(TAttribute));
		}
		public static IEnumerable<ICommonAttribute> GetAttributes(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.TypeTypeToAttributes.TryGetValue(attributeType, out var commonAttributes) ? commonAttributes : Enumerable.Empty<ICommonAttribute>();
		}
		public static IEnumerable<ICommonAttribute> GetAttributes(this IHasAttributes hasAttributes, IHasType attributeHasType) {
			return hasAttributes.GetAttributes(attributeHasType.Type);
		}
		public static IEnumerable<ICommonAttribute> GetAttributes(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.TypeFullNameToAttributes.TryGetValue(attributeTypeFullName, out var commonAttributes) ? commonAttributes : Enumerable.Empty<ICommonAttribute>();
		}

		public static bool ContainsAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.ContainsAttribute(typeof(TAttribute));
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.TypeTypeToAttributes.ContainsKey(attributeType);
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, IHasType attributeHasType) {
			return hasAttributes.ContainsAttribute(attributeHasType.Type);
		}
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.TypeFullNameToAttributes.ContainsKey(attributeTypeFullName);
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