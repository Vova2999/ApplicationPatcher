using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasAttributesExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static TAttribute GetReflectionAttribute<TAttribute>(this IHasAttributes hasAttributes, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)hasAttributes.GetAttribute<TAttribute>(throwExceptionIfNotFound)?.Reflection;
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonAttribute GetAttribute<TAttribute>(this IHasAttributes hasAttributes, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return hasAttributes.GetAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, Type attributeType, bool throwExceptionIfNotFound = false) {
			return hasAttributes.Attributes.CheckLoaded().SingleOrDefault(attribute => attribute.Is(attributeType), throwExceptionIfNotFound, attributeType.FullName);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName, bool throwExceptionIfNotFound = false) {
			return hasAttributes.Attributes.CheckLoaded().SingleOrDefault(attribute => attribute.Is(attributeTypeFullName), throwExceptionIfNotFound, attributeTypeFullName);
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<CommonAttribute> GetAttributes<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.GetAttributes(typeof(TAttribute));
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Attributes.CheckLoaded().Where(attribute => attribute.Is(attributeType));
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Attributes.CheckLoaded().Where(attribute => attribute.Is(attributeTypeFullName));
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static bool ContainsAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.ContainsAttribute(typeof(TAttribute));
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Attributes.CheckLoaded().Any(attribute => attribute.Is(attributeType));
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Attributes.CheckLoaded().Any(attribute => attribute.Is(attributeTypeFullName));
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static bool NotContainsAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return !hasAttributes.ContainsAttribute<TAttribute>();
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return !hasAttributes.ContainsAttribute(attributeType);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return !hasAttributes.ContainsAttribute(attributeTypeFullName);
		}
	}
}