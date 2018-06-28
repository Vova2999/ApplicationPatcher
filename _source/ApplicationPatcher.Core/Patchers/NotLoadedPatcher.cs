﻿using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Types.CommonMembers;

namespace ApplicationPatcher.Core.Patchers {
	public abstract class NotLoadedPatcher : IPatcher {
		[AddLogOffset]
		public abstract PatchResult Patch(CommonAssembly assembly);
	}
}