using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Helpers;

namespace ApplicationPatcher.Core.Extensions {
	public static class EnumerableExtensions {
		[DoNotAddLogOffset]
		public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> actionOnValue) {
			foreach (var value in values)
				actionOnValue(value);
		}
	}
}