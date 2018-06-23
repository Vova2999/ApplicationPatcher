using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class StringExtensions {
		[ContractAnnotation("null => true; notnull => false")]
		public static bool IsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}

		public static string EmptyIfNull(this string str) {
			return str ?? string.Empty;
		}
	}
}