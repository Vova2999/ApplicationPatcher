using System;
using System.Linq;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasFieldsExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonField GetField(this IHasFields hasFields, string fieldName, bool throwExceptionIfNotFound = false) {
			return hasFields.Fields.CheckLoaded().SingleOrDefault(field => field.Name == fieldName, throwExceptionIfNotFound, fieldName);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonField[] GetFields(this IHasFields hasFields, Type parameterType) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterType)).ToArray();
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonField[] GetFields(this IHasFields hasFields, string parameterTypeFullName) {
			return hasFields.Fields.CheckLoaded().Where(field => field.Is(parameterTypeFullName)).ToArray();
		}
	}
}