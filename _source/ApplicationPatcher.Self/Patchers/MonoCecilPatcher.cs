using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self.Patchers {
	[UsedImplicitly]
	public class MonoCecilPatcher : IPatcher {
		private readonly Log log;

		public MonoCecilPatcher() {
			log = Log.For(this);
		}

		[DoNotAddLogOffset]
		public void Patch(CommonAssembly assembly) {
			log.Info("Patching sealed types...");
			var sealedTypes = assembly.TypesFromThisAssembly.Where(x => x.MonoCecilType.IsSealed).ToArray();

			if (!sealedTypes.Any()) {
				log.Debug("Not found sealed types");
				return;
			}

			log.Debug("Sealed types found:", sealedTypes.Select(viewModel => viewModel.FullName).OrderBy(fullName => fullName));

			log.Info("Patching sealed types...");
			sealedTypes.ForEach(type => type.MonoCecilType.IsSealed = false);
			log.Info("Sealed types was patched");
		}
	}
}