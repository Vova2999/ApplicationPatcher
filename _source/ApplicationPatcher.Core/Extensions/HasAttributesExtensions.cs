using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasAttributesExtensions {
		[UsedImplicitly]
		public static TAttribute GetReflectionAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return (TAttribute)hasAttributes.GetAttribute<TAttribute>()?.ReflectionAttribute;
		}

		[UsedImplicitly]
		public static Attribute GetReflectionAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.GetAttribute(attributeTypeFullName)?.ReflectionAttribute;
		}

		[UsedImplicitly]
		public static CommonAttribute GetAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.Attributes.SingleOrDefault(attribute => attribute.Is(typeof(TAttribute)));
		}

		[UsedImplicitly]
		public static CommonAttribute GetAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Attributes.SingleOrDefault(attribute => attribute.Type.FullName == attributeTypeFullName);
		}

		[UsedImplicitly]
		public static IEnumerable<CommonAttribute> GetAttributes<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return hasAttributes.Attributes.Where(attribute => attribute.Is(typeof(TAttribute)));
		}

		[UsedImplicitly]
		public static IEnumerable<CommonAttribute> GetAttributes(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Attributes.Where(attribute => attribute.Type.FullName == attributeTypeFullName);
		}

		[UsedImplicitly]
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Attributes.Any(attribute => attribute.Is(attributeType));
		}

		[UsedImplicitly]
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return hasAttributes.Attributes.Any(attribute => attribute.Type.FullName == attributeTypeFullName);
		}

		[UsedImplicitly]
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return !hasAttributes.ContainsAttribute(attributeType);
		}

		[UsedImplicitly]
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, string attributeTypeFullName) {
			return !hasAttributes.ContainsAttribute(attributeTypeFullName);
		}
	}
}