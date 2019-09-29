using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasFieldsExtensions {
		public static bool TryGetField(this IHasFields hasFields, string fieldName, out ICommonField foundCommonField) {
			return (foundCommonField = hasFields.GetField(fieldName)) != null;
		}
		public static ICommonField GetField(this IHasFields hasFields, string fieldName, bool throwExceptionIfNotFound = false) {
			return (hasFields.FieldNameToFields.CheckLoaded().TryGetValue(fieldName, out var commonFields) ? commonFields : Enumerable.Empty<ICommonField>()).SingleOrDefault(throwExceptionIfNotFound, fieldName);
		}

		public static IEnumerable<ICommonField> GetFields(this IHasFields hasFields, IHasType hasType) {
			return hasFields.GetFields(hasType.Type);
		}
		public static IEnumerable<ICommonField> GetFields(this IHasFields hasFields, Type parameterType) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterType));
		}
		public static IEnumerable<ICommonField> GetFields(this IHasFields hasFields, string parameterTypeFullName) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterTypeFullName));
		}
	}
}