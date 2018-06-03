using System;
using System.Collections.Generic;

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonBaseExtensions {
		public static IEnumerable<TCommonBase> CheckLoaded<TCommonBase>(this IEnumerable<TCommonBase> commonBases) {
			return commonBases ?? throw new InvalidOperationException("Not loaded");
		}
	}
}