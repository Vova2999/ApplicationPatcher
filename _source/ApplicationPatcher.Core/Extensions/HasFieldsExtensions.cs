using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasFieldsExtensions {
		public static CommonField GetField(this IHasFields hasFields, string fieldName, bool throwExceptionIfNotFound = false) {
			return hasFields.Fields.CheckLoaded().SingleOrDefault(field => field.Name == fieldName, throwExceptionIfNotFound, fieldName);
		}
	}
}