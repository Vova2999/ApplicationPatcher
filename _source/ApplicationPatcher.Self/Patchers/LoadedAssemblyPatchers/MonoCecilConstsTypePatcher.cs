﻿using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self.Patchers.LoadedAssemblyPatchers {
	[UsedImplicitly]
	public class MonoCecilConstsTypePatcher : LoadedAssemblyPatcher {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly Log log;

		public MonoCecilConstsTypePatcher(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public override PatchResult Patch(CommonAssembly assembly) {
			const string constsTypeFullName = "Consts";
			log.Info($"Patching '{constsTypeFullName}' type...");

			var constsType = assembly.GetCommonType(constsTypeFullName, false);

			if (constsType == null) {
				log.Info($"Not found '{constsTypeFullName}' type");
				return PatchResult.Succeeded;
			}

			constsType.Load().GetField("PublicKey").MonoCecilField.Constant = applicationPatcherSelfConfiguration.MonoCecilNewPublicKey.ToHexString();

			log.Info($"{constsTypeFullName} type was patched");
			return PatchResult.Succeeded;
		}
	}
}