using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasFieldsExtensions {
		public static bool TryGetField(this IHasFields hasFields, string fieldName, out CommonField foundCommonField) {
			return (foundCommonField = hasFields.GetField(fieldName)) != null;
		}
		public static CommonField GetField(this IHasFields hasFields, string fieldName, bool throwExceptionIfNotFound = false) {
			return (hasFields.Load().FieldNameToField.TryGetValue(fieldName, out var commonFields) ? commonFields : Enumerable.Empty<CommonField>()).SingleOrDefault(throwExceptionIfNotFound, fieldName);
		}

		public static IEnumerable<CommonField> GetFields(this IHasFields hasFields, IHasType hasType) {
			return hasFields.GetFields(hasType.Type);
		}
		public static IEnumerable<CommonField> GetFields(this IHasFields hasFields, Type parameterType) {
			return hasFields.Load().Fields.Where(field => field.Is(parameterType));
		}
		public static IEnumerable<CommonField> GetFields(this IHasFields hasFields, string parameterTypeFullName) {
			return hasFields.Load().Fields.Where(field => field.Is(parameterTypeFullName));
		}
	}
}