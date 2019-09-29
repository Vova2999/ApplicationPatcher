using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Self.Patchers.OnLoadedAssembly {
	public class MonoCecilConstsTypePatcher : PatcherOnLoadedApplication {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

		public MonoCecilConstsTypePatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(ICommonAssembly assembly) {
			const string constsTypeFullName = "Consts";
			log.Info($"Patching '{constsTypeFullName}' type...");

			var constsType = assembly.GetCommonTypeFromThisAssembly(constsTypeFullName);

			if (constsType == null) {
				log.Info($"Not found '{constsTypeFullName}' type");
				return PatchResult.Continue;
			}

			constsType.GetField("PublicKey").MonoCecil.Constant = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey.ToHexString();

			log.Info($"'{constsTypeFullName}' type was patched");
			return PatchResult.Continue;
		}
	}
}