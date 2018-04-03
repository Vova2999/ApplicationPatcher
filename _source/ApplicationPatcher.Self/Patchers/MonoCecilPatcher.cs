﻿using System.Linq;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Self.Patchers {
	public class MonoCecilPatcher : IPatcher {
		private readonly Log log;

		public MonoCecilPatcher() {
			log = Log.For(this);
		}

		public void Patch(CommonAssembly assembly) {
			log.Info("Patching sealed types...");
			var sealedTypes = assembly.TypesFromThisAssembly.Where(x => x.MonoCecilType.IsSealed).ToArray();

			if (!sealedTypes.Any()) {
				log.Debug("Not found sealed types");
				return;
			}

			log.Debug("Sealed types found:", sealedTypes.Select(viewModel => viewModel.FullName));
			sealedTypes.ForEach(type => type.MonoCecilType.IsSealed = false);

			log.Info("Sealed types was patched");
		}
	}
}