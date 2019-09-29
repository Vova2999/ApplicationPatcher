using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.BaseInterfaces;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class HasPropertiesExtensions {
		public static bool TryGetProperty(this IHasProperties hasProperties, string propertyName, out ICommonProperty foundCommonProperty) {
			return (foundCommonProperty = hasProperties.GetProperty(propertyName)) != null;
		}
		public static ICommonProperty GetProperty(this IHasProperties hasProperties, string propertyName, bool throwExceptionIfNotFound = false) {
			return (hasProperties.PropertyNameToProperties.CheckLoaded().TryGetValue(propertyName, out var commonProperties) ? commonProperties : Enumerable.Empty<ICommonProperty>()).SingleOrDefault(throwExceptionIfNotFound, propertyName);
		}

		public static IEnumerable<ICommonProperty> GetProperties(this IHasProperties hasProperties, IHasType hasType) {
			return hasProperties.GetProperties(hasType.Type);
		}
		public static IEnumerable<ICommonProperty> GetProperties(this IHasProperties hasProperties, Type propertyType) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyType));
		}
		public static IEnumerable<ICommonProperty> GetProperties(this IHasProperties hasProperties, string propertyTypeFullName) {
			return hasProperties.Properties.CheckLoaded().Where(property => property.Is(propertyTypeFullName));
		}
	}
}