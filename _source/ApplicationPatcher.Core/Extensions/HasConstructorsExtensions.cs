using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Extensions {
	public static class HasConstructorsExtensions {
		public static CommonConstructor GetConstructor(this IHasConstructors hasConstructors, Type[] constructorParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasConstructors.Constructors.CheckLoaded().SingleOrDefault(constructor => constructor.ParameterTypes.SequenceEqual(constructorParameterTypes), throwExceptionIfNotFound, ".ctor");
		}
	}
}