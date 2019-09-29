using System;
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
	}
}