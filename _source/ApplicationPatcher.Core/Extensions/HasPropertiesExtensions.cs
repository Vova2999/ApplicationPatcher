using System;
using System.Linq;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasPropertiesExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonProperty GetProperty(this IHasProperties hasProperties, string propertyName, bool throwExceptionIfNotFound = false) {
			return hasProperties.Properties.CheckLoaded().SingleOrDefault(property => property.Name == propertyName, throwExceptionIfNotFound, propertyName);
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, Type propertyType) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyType)).ToArray();
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, string propertyTypeFullName) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyTypeFullName)).ToArray();
		}
	}
}