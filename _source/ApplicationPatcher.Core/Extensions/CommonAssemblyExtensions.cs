using System;
using ApplicationPatcher.Core.Types.Common;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly hasTypes, Type type, bool throwExceptionIfNotFound = false) {
			return hasTypes.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(type), throwExceptionIfNotFound, type.FullName);
		}
		public static CommonType GetCommonTypeFromThisAssembly(this CommonAssembly hasTypes, string typeFullName, bool throwExceptionIfNotFound = false) {
			return hasTypes.TypesFromThisAssembly.SingleOrDefault(commonType => commonType.Is(typeFullName), throwExceptionIfNotFound, typeFullName);
		}
	}
}