using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Core.Extensions {
	internal static class PatchExtensions {
		internal static PatchResult PatchApplication(this IEnumerable<IPatcher> patchers, ICommonAssembly assembly, ILog log) {
			if (patchers == null)
				return PatchResult.Continue;

			foreach (var patcher in patchers) {
				log.Info($"Apply '{patcher.GetType().FullName}' patcher...");
				var patchResult = patcher.Patch(assembly);
				log.Info($"Patcher '{patcher.GetType().FullName}' was applied with result: {patchResult}");

				switch (patchResult) {
					case PatchResult.Continue:
						continue;
					case PatchResult.Cancel:
						log.Info("Patching application was canceled");
						return PatchResult.Cancel;
					default:
						throw new ArgumentOutOfRangeException(nameof(patchResult));
				}
			}

			return PatchResult.Continue;
		}
	}
}