using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	[UsedImplicitly]
	public class MonoCecilSelectedTypesPatcher : ILoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly Log log;

		public MonoCecilSelectedTypesPatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public PatchResult Patch(CommonAssembly assembly) {
			log.Info("Patching selected types...");
			var foundedSelectedPatchingTypes = assembly.TypesFromThisAssembly
				.Where(type => applicationPatcherSelfConfiguration.MonoCecilSelectedPatchingTypeFullNames.Contains(type.FullName))
				.ToArray();

			if (!foundedSelectedPatchingTypes.Any()) {
				log.Info("Not found selected types");
				return PatchResult.Succeeded;
			}

			log.Debug("Selected types found:", foundedSelectedPatchingTypes.Select(viewModel => viewModel.FullName).OrderBy(fullName => fullName));

			foundedSelectedPatchingTypes.ForEach(type => type.Load().MonoCecilType.IsSealed = false);
			log.Info("Sealed types was patched");
			return PatchResult.Succeeded;
		}
	}
}