using System;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasTypeExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static bool Is(this IHasType hasType, Type type) {
			return hasType.Type == type;
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool Is(this IHasType hasType, string typeFullName) {
			return hasType.Type.FullName == typeFullName;
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static bool IsNot(this IHasType hasType, Type type) {
			return !hasType.Is(type);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool IsNot(this IHasType hasType, string typeFullName) {
			return !hasType.Is(typeFullName);
		}


		[UsedImplicitly, DoNotAddLogOffset]
		public static bool IsInheritedFrom(this IHasType hasType, Type type) {
			return type.IsAssignableFrom(hasType.Type);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static bool IsNotInheritedFrom(this IHasType hasType, Type type) {
			return !hasType.IsInheritedFrom(type);
		}
	}
}