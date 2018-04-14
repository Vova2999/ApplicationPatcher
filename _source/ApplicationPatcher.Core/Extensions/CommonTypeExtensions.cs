using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	public static class CommonTypeExtensions {
		[UsedImplicitly]
		public static IEnumerable<CommonType> WhereFrom(this IEnumerable<CommonType> commonTypes, CommonAssembly commonAssembly) {
			return commonTypes.Where(commonType => commonType.MonoCecilType.Module == commonAssembly.MainMonoCecilAssembly.MainModule);
		}
	}
}