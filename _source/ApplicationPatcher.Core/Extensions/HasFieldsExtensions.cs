using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasFieldsExtensions {
		public static CommonField GetField(this IHasFields hasFields, string fieldName) {
			return hasFields.Fields.FirstOrDefault(field => field.Name == fieldName);
		}
	}
}