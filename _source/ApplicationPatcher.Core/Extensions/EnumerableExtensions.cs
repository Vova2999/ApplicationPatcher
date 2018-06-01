using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class EnumerableExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> actionOnValue) {
			foreach (var value in values)
				actionOnValue(value);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static TValue SingleOrDefault<TValue>(this IEnumerable<TValue> values, Func<TValue, bool> predicate, bool throwExceptionIfNotFound, string valueFullName) {
			var count = 0;
			var tempValue = default(TValue);
			foreach (var value in values.Where(predicate)) {
				tempValue = value;
				count++;
			}

			switch (count) {
				case 0:
					return throwExceptionIfNotFound ? throw new InvalidOperationException($"Not found '{valueFullName}'") : tempValue;
				case 1:
					return tempValue;
				default:
					throw new InvalidOperationException($"Found more than one '{valueFullName}'");
			}
		}
	}
}