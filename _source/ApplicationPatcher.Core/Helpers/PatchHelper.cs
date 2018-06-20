using System;
using System.Collections.Generic;
using ApplicationPatcher.Core.Logs;

namespace ApplicationPatcher.Core.Helpers {
	public static class PatchHelper {
		public static PatchResult PatchApplication<TPatcher>(IEnumerable<TPatcher> patchers, Func<TPatcher, PatchResult> patch, ILog log) {
			if (patchers == null)
				return PatchResult.Continue;

			foreach (var patcher in patchers) {
				log.Info($"Apply '{patcher.GetType().FullName}' patcher...");
				var patchResult = patch(patcher);
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