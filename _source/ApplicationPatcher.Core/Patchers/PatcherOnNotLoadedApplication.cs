﻿using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Types.CommonInterfaces;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class PatcherOnNotLoadedApplication : IPatcher {
		[AddLogOffset]
		public abstract PatchResult Patch(ICommonAssembly assembly);
	}
}