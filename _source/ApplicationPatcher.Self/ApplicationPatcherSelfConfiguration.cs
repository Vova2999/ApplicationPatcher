using ApplicationPatcher.Core;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfConfiguration : ConfigurationFile<ApplicationPatcherSelfConfiguration> {
		protected override string ConfigurationFileName => "ApplicationPatcher.Self.config.xml";

		public byte[] MonoCecilNewPublicKey { get; set; }
		public byte[] MonoCecilNewPublicKeyToken { get; set; }
		public string MonoCecilResultDirectoryName { get; set; }
		public string MonoCecilSignatureFileName { get; set; }
		public string[] MonoCecilApplicationFileNames { get; set; }
		public string[] MonoCecilOutputDirectories { get; set; }
		public string[] MonoCecilSelectedAssemblyReferenceNames { get; set; }
		public string[] MonoCecilSelectedInternalsVisibleToAttributeNames { get; set; }
		public string[] MonoCecilSelectedPatchingTypeFullNames { get; set; }
	}
}