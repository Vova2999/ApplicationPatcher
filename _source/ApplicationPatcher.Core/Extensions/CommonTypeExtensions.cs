using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonTypeExtensions {
		public static IEnumerable<CommonType> WhereFrom(this IEnumerable<CommonType> commonTypes, CommonAssembly commonAssembly) {
			return commonTypes.CheckLoaded().Where(commonType => commonType.MonoCecil.Module == commonAssembly.MonoCecil.MainModule);
		}
	}
}