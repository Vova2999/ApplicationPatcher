using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class StringExtensions {
		[ContractAnnotation("null => true")]
		public static bool IsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}

		[ContractAnnotation("null => false")]
		public static bool IsSignificant(this string str) {
			return !string.IsNullOrEmpty(str);
		}

		public static string NullIfEmpty(this string str) {
			return str.IsNullOrEmpty() ? null : str;
		}

		public static string EmptyIfNull(this string str) {
			return str ?? string.Empty;
		}
	}
}