﻿using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonAssemblyExtensions {
		[UsedImplicitly]
		public static IEnumerable<CommonType> GetInheritanceCommonTypes(this CommonAssembly commonAssembly, Type baseType) {
			return commonAssembly.Types.Where(type => type.IsInheritedFrom(baseType));
		}

		[UsedImplicitly]
		public static CommonType GetCommonType(this CommonAssembly commonAssembly, Type type) {
			return commonAssembly.Types?.FirstOrDefault(commonType => commonType.Is(type)) ?? throw new ArgumentException($"Not found type '{type.FullName}'");
		}
	}
}