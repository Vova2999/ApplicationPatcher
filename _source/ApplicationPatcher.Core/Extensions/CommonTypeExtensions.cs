using System.Collections.Generic;
using System.Linq;
using ApplicationPatcher.Core.Types.CommonInterfaces;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core.Extensions {
	[PublicAPI]
	public static class CommonTypeExtensions {
		public static IEnumerable<ICommonType> WhereFrom(this IEnumerable<ICommonType> commonTypes, ICommonAssembly commonAssembly) {
			return commonTypes.CheckLoaded().Where(commonType => commonType.MonoCecil.Module == commonAssembly.MonoCecil.MainModule);
		}
	}
}