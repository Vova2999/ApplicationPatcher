using System;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasTypeExtensions {
		[UsedImplicitly]
		public static bool Is(this IHasType hasType, Type type) {
			return hasType.Type == type;
		}

		[UsedImplicitly]
		public static bool IsNot(this IHasType hasType, Type type) {
			return !hasType.Is(type);
		}

		[UsedImplicitly]
		public static bool IsInheritedFrom(this IHasType hasType, Type type) {
			return type.IsAssignableFrom(hasType.Type);
		}

		[UsedImplicitly]
		public static bool IsNotInheritedFrom(this IHasType hasType, Type type) {
			return !hasType.IsInheritedFrom(type);
		}
	}
}