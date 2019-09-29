using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class CommonConstructorExtensions {
		public static IEnumerable<TAttribute> GetReflectionAttributes<TAttribute>(this ICommonConstructor commonConstructor) where TAttribute : Attribute {
			return commonConstructor.Reflection.GetCustomAttributes<TAttribute>(false);
		}
		public static bool TryGetReflectionAttribute<TAttribute>(this ICommonConstructor commonConstructor, out TAttribute foundAttribute) where TAttribute : Attribute {
			return (foundAttribute = commonConstructor.GetReflectionAttribute<TAttribute>()) != null;
		}
		public static TAttribute GetReflectionAttribute<TAttribute>(this ICommonConstructor commonConstructor, bool throwExceptionIfNotFound = false) where TAttribute : Attribute {
			return (TAttribute)commonConstructor.GetReflectionAttribute(typeof(TAttribute), throwExceptionIfNotFound);
		}
		public static Attribute GetReflectionAttribute(this ICommonConstructor commonConstructor, Type attributeType, bool throwExceptionIfNotFound = false) {
			return (Attribute)commonConstructor.Reflection.GetCustomAttributes(attributeType, false).SingleOrDefault(throwExceptionIfNotFound, attributeType.FullName);
		}

		public static bool ContainsReflectionAttribute<TAttribute>(this ICommonConstructor commonConstructor) where TAttribute : Attribute {
			return commonConstructor.ContainsReflectionAttribute(typeof(TAttribute));
		}
		public static bool ContainsReflectionAttribute(this ICommonConstructor commonConstructor, Type attributeType) {
			return commonConstructor.Reflection.GetCustomAttributes(attributeType, false).Any();
		}
		public static bool ContainsReflectionAttribute(this ICommonConstructor commonConstructor, IHasType attributeHasType) {
			return commonConstructor.ContainsReflectionAttribute(attributeHasType.Type);
		}

		public static bool NotContainsReflectionAttribute<TAttribute>(this ICommonConstructor commonConstructor) where TAttribute : Attribute {
			return !commonConstructor.ContainsReflectionAttribute<TAttribute>();
		}
		public static bool NotContainsReflectionAttribute(this ICommonConstructor commonConstructor, Type attributeType) {
			return !commonConstructor.ContainsReflectionAttribute(attributeType);
		}
		public static bool NotContainsReflectionAttribute(this ICommonConstructor commonConstructor, IHasType attributeHasType) {
			return !commonConstructor.ContainsReflectionAttribute(attributeHasType);
		}
	}
}