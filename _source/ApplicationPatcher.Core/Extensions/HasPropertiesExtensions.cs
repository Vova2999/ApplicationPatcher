using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasPropertiesExtensions {
		public static CommonProperty GetProperty(this IHasProperties hasProperties, string propertyName, bool throwExceptionIfNotFound = false) {
			return hasProperties.Properties.CheckLoaded().SingleOrDefault(property => property.Name == propertyName, throwExceptionIfNotFound, propertyName);
		}

		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, Type propertyType) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyType)).ToArray();
		}

		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, string propertyTypeFullName) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyTypeFullName)).ToArray();
		}
	}
}