using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class CommonMethodExtensions {
		public static IEnumerable<TAttribute> GetReflectionAttributes<TAttribute>(this ICommonMethod commonMethod) where TAttribute : Attribute {
			return commonMethod.Reflection.GetCustomAttributes<TAttribute>(false);
		}
		public static bool TryGetReflectionAttribute<TAttribute>(this ICommonMethod commonMethod, out TAttribute foundAttribute) where TAttribute : Attribute {
			return (foundAttribute = commonMethod.GetReflectionAttribute<TAttribute>()) != null;
		}
		public static TAttribute GetReflectionAttribute<TAttribute>(this ICommonMethod commonMethod, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)commonMethod.GetReflectionAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static Attribute GetReflectionAttribute(this ICommonMethod commonMethod, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (Attribute)commonMethod.Reflection.GetCustomAttributes(attributeType, false).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}

		public static bool ContainsReflectionAttribute<TAttribute>(this ICommonMethod commonMethod) where TAttribute : Attribute {
			return commonMethod.ContainsReflectionAttribute(typeof(TAttribute));
		}
		public static bool ContainsReflectionAttribute(this ICommonMethod commonMethod, Type attributeType) {
			return commonMethod.Reflection.GetCustomAttributes(attributeType, false).Any();
		}
		public static bool ContainsReflectionAttribute(this ICommonMethod commonMethod, IHasType attributeHasType) {
			return commonMethod.ContainsReflectionAttribute(attributeHasType.Type);
		}

		public static bool NotContainsReflectionAttribute<TAttribute>(this ICommonMethod commonMethod) where TAttribute : Attribute {
			return !commonMethod.ContainsReflectionAttribute<TAttribute>();
		}
		public static bool NotContainsReflectionAttribute(this ICommonMethod commonMethod, Type attributeType) {
			return !commonMethod.ContainsReflectionAttribute(attributeType);
		}
		public static bool NotContainsReflectionAttribute(this ICommonMethod commonMethod, IHasType attributeHasType) {
			return !commonMethod.ContainsReflectionAttribute(attributeHasType);
		}
	}
}