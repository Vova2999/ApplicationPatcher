﻿using System;
using System.Linq;
using ApplicationPatcher.Core.Types.Base;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class HasMethodsExtensions {
		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, Type[] methodParameterTypes, bool throwExceptionIfNotFound = false) {
			return hasMethods.Methods.CheckLoaded().SingleOrDefault(method => method.Name == methodName && method.ParameterTypes.SequenceEqual(methodParameterTypes ?? Type.EmptyTypes), throwExceptionIfNotFound, methodName);
		}

		public static CommonMethod GetMethod(this IHasMethods hasMethods, string methodName, string[] methodParameterTypeFullNames, bool throwExceptionIfNotFound = false) {
			return hasMethods.Methods.CheckLoaded().SingleOrDefault(method => method.Name == methodName && method.ParameterTypes.Select(type => type.FullName).SequenceEqual(methodParameterTypeFullNames ?? new string[0]), throwExceptionIfNotFound, ".ctor");
		}

		public static CommonMethod[] GetMethods(this IHasMethods hasMethods, string methodName) {
			return hasMethods.Methods.CheckLoaded().Where(method => method.Name == methodName).ToArray();
		}
	}
}