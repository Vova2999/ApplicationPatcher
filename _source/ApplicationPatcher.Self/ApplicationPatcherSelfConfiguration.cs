﻿using ApplicationPatcher.Core;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfConfiguration : ConfigurationFile<ApplicationPatcherSelfConfiguration> {
		protected override string ConfigurationFileName => "ApplicationPatcher.Self.config.xml";

		[UsedImplicitly]
		public string ApplicationPatcherCoreDirectoryName { get; set; }

		[UsedImplicitly]
		public string MonoCecilResultDirectoryName { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilApplicationNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedAssemblyReferenceNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedInternalsVisibleToAttributeNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedPatchingTypeFullNames { get; set; }
	}
}