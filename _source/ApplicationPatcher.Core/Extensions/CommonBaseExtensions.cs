using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonBaseExtensions {
		[UsedImplicitly, DoNotAddLogOffset]
		public static IEnumerable<TCommonBase> CheckLoaded<TCommonBase>(this IEnumerable<TCommonBase> commonBases) {
			return commonBases ?? throw new InvalidOperationException("Not loaded");
		}
	}
}