using System;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonMembers;
using ApplicationPatcher.Core.Types.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasPropertiesExtensions {
		public static bool TryGetProperty(this IHasProperties hasProperties, string propertyName, out CommonProperty foundCommonProperty) {
			return (foundCommonProperty = hasProperties.GetProperty(propertyName)) != null;
		}
		public static CommonProperty GetProperty(this IHasProperties hasProperties, string propertyName, bool throwExceptionIfNotFound = false) {
			return hasProperties.Load().Properties.SingleOrDefault(property => property.Name == propertyName, throwExceptionIfNotFound, propertyName);
		}

		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, IHasType hasType) {
			return hasProperties.GetProperties(hasType.Type);
		}
		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, Type propertyType) {
			return hasProperties.Load().Properties.Where(property => property.Is(propertyType)).ToArray();
		}
		public static CommonProperty[] GetProperties(this IHasProperties hasProperties, string propertyTypeFullName) {
			return hasProperties.Load().Properties.Where(property => property.Is(propertyTypeFullName)).ToArray();
		}
	}
}