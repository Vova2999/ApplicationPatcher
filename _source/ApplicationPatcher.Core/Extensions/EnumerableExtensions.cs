using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class EnumerableExtensions {
		public static string JoinToString<TValue>(this IEnumerable<TValue> values, string separator) {
			return string.Join(separator, values);
		}

		public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> actionOnValue) {
			foreach (var value in values)
				actionOnValue(value);
		}

		public static void ForEach<TValue, TResult>(this IEnumerable<TValue> values, Func<TValue, TResult> actionOnValue) {
			foreach (var value in values)
				actionOnValue(value);
		}

		public static TValue SingleOrDefault<TValue>(this IEnumerable<TValue> values, bool throwExceptionIfNotFound, string valueFullName) {
			return SingleOrDefault(values, value => true, throwExceptionIfNotFound, valueFullName);
		}

		public static TValue SingleOrDefault<TValue>(this IEnumerable<TValue> values, Func<TValue, bool> predicate, bool throwExceptionIfNotFound, string valueFullName) {
			var count = 0;
			var tempValue = default(TValue);
			foreach (var value in values.Where(predicate)) {
				tempValue = value;
				count++;

				if (count > 1)
					break;
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

		internal static TResultCollection CheckLoaded<TResultCollection>(this TResultCollection collection) where TResultCollection : IEnumerable {
			return Equals(collection, null) ? throw new InvalidOperationException("This object not loaded") : collection;
		}
	}
}