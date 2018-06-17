using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	public class MonoCecilConstsTypePatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

		public MonoCecilConstsTypePatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			const string constsTypeFullName = "Consts";
			log.Info($"Patching '{constsTypeFullName}' type...");

			var constsType = assembly.GetCommonTypeFromThisAssembly(constsTypeFullName);

			if (constsType == null) {
				log.Info($"Not found '{constsTypeFullName}' type");
				return PatchResult.Succeeded;
			}

			constsType.Load().GetField("PublicKey").MonoCecil.Constant = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey.ToHexString();

			log.Info($"'{constsTypeFullName}' type was patched");
			return PatchResult.Succeeded;
		}
	}
}