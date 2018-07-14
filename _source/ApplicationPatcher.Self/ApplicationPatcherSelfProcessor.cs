using System;
using System.Collections.Generic;
using System.IO;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Logs;

// ReSharper disable ClassNeverInstantiated.Global

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfProcessor {
		private readonly ApplicationPatcherProcessor applicationPatcherProcessor;
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ILog log;

		public ApplicationPatcherSelfProcessor(ApplicationPatcherProcessor applicationPatcherProcessor, ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration) {
			this.applicationPatcherProcessor = applicationPatcherProcessor;
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			log = Log.For(this);
		}

		public void PatchSelfApplication() {
			log.Info("Patching all mono cecil applications...");

			using (CurrentDirectoryHelper.FromExecutingAssembly()) {
				var monoCecilApplicationResultNames = ShiftMonoCecilApplications(applicationPatcherSelfConfiguration.MonoCecilApplicationFileNames, applicationPatcherSelfConfiguration.MonoCecilResultDirectoryName, false);

				foreach (var monoCecilResultApplicationName in monoCecilApplicationResultNames) {
					log.Info($"Patching '{monoCecilResultApplicationName}' application...");
					PatchApplication(applicationPatcherProcessor, monoCecilResultApplicationName, applicationPatcherSelfConfiguration.MonoCecilSignatureFileName);
					log.Info($"Application '{monoCecilResultApplicationName}' was patched");
				}

				log.Info("All mono cecil applications was patched");

				foreach (var monoCecilOutputDirectory in applicationPatcherSelfConfiguration.MonoCecilOutputDirectories)
					ShiftMonoCecilApplications(monoCecilApplicationResultNames, monoCecilOutputDirectory, true);
			}
		}

		private string[] ShiftMonoCecilApplications(IEnumerable<string> monoCecilApplicationNames, string resultDirectoryName, bool overwrite) {
			var resultDirectoryPath = Path.GetFullPath(resultDirectoryName);
			log.Info($"Shifting mono cecil applications to '{resultDirectoryPath}' directory...");

			if (!Directory.Exists(resultDirectoryPath)) {
				log.Debug($"Create '{resultDirectoryPath}' directory");
				Directory.CreateDirectory(resultDirectoryPath);
			}

			var monoCecilApplicationResultNames = new List<string>();
			foreach (var monoCecilApplicationName in monoCecilApplicationNames) {
				var monoCecilApplicationResultName = Path.Combine(resultDirectoryPath, Path.GetFileName(monoCecilApplicationName) ?? throw new Exception());
				monoCecilApplicationResultNames.Add(monoCecilApplicationResultName);

				if (!File.Exists(monoCecilApplicationResultName))
					File.Copy(monoCecilApplicationName, monoCecilApplicationResultName);
				else if (overwrite)
					File.Copy(monoCecilApplicationName, monoCecilApplicationResultName, true);
			}

			log.Info("Mono cecil applications was shifted");
			return monoCecilApplicationResultNames.ToArray();
		}

		[AddLogOffset]
		private static void PatchApplication(ApplicationPatcherProcessor applicationPatcherProcessor, string applicationPath, string signaturePath) {
			applicationPatcherProcessor.PatchApplication(applicationPath, signaturePath);
		}
	}
}