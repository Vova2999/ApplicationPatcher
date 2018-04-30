using ApplicationPatcher.Core;
using JetBrains.Annotations;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfConfiguration : ConfigurationFile<ApplicationPatcherSelfConfiguration> {
		protected override string ConfigurationFileName => "ApplicationPatcher.Self.config.xml";

		[UsedImplicitly]
		public byte[] MonoCecilNewPublicKey { get; set; }

		[UsedImplicitly]
		public byte[] MonoCecilNewPublicKeyToken { get; set; }

		[UsedImplicitly]
		public string ApplicationPatcherCoreDirectoryName { get; set; }

		[UsedImplicitly]
		public string MonoCecilResultDirectoryName { get; set; }

		[UsedImplicitly]
		public string MonoCecilSignatureFileName { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilApplicationFileNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedAssemblyReferenceNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedInternalsVisibleToAttributeNames { get; set; }

		[UsedImplicitly]
		public string[] MonoCecilSelectedPatchingTypeFullNames { get; set; }
	}
}