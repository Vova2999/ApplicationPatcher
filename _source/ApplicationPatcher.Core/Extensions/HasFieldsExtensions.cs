﻿using System;
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
			return hasFields.Load().Fields.SingleOrDefault(field => field.Name == fieldName, throwExceptionIfNotFound, fieldName);
		}

		public static CommonField[] GetFields(this IHasFields hasFields, IHasType hasType) {
			return hasFields.GetFields(hasType.Type);
		}
		public static CommonField[] GetFields(this IHasFields hasFields, Type parameterType) {
			return hasFields.Load().Fields.Where(field => field.Is(parameterType)).ToArray();
		}
		public static CommonField[] GetFields(this IHasFields hasFields, string parameterTypeFullName) {
			return hasFields.Load().Fields.Where(field => field.Is(parameterTypeFullName)).ToArray();
		}
	}
}