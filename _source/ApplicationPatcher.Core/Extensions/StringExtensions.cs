using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class StringExtensions {
		[ContractAnnotation("null => true; notnull => false")]
		public static bool IsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}
	}
}