using System;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasFieldsExtensions {
		public static CommonField GetField(this IHasFields hasFields, string fieldName, bool throwExceptionIfNotFound = false) {
			return hasFields.Fields.CheckLoaded().SingleOrDefault(field => field.Name == fieldName, throwExceptionIfNotFound, fieldName);
		}

		public static CommonField[] GetFields(this IHasFields hasFields, IHasType hasType) {
			return hasFields.GetFields(hasType.Type);
		}
		public static CommonField[] GetFields(this IHasFields hasFields, Type parameterType) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterType)).ToArray();
		}
		public static CommonField[] GetFields(this IHasFields hasFields, string parameterTypeFullName) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterTypeFullName)).ToArray();
		}
	}
}