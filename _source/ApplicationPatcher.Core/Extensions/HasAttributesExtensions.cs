using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasAttributesExtensions {
		[UsedImplicitly]
		public static TAttribute GetReflectionAttribute<TAttribute>(this IHasAttributes hasAttributes) where TAttribute : Attribute {
			return (TAttribute)hasAttributes.Attributes.FirstOrDefault(attribute => attribute.Type == typeof(TAttribute))?.ReflectionAttribute;
		}

		[UsedImplicitly]
		public static bool ContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return hasAttributes.Attributes.Any(attribute => attribute.Type == attributeType);
		}

		[UsedImplicitly]
		public static bool NotContainsAttribute(this IHasAttributes hasAttributes, Type attributeType) {
			return !hasAttributes.ContainsAttribute(attributeType);
		}
	}
}