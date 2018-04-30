using System;
using System.Collections.Generic;
using System.IO;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Helpers;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfProcessor {
		private readonly ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration;
		private readonly ApplicationPatcherProcessor applicationPatcherProcessor;
		private readonly Log log;

		public ApplicationPatcherSelfProcessor(ApplicationPatcherSelfConfiguration applicationPatcherSelfConfiguration, ApplicationPatcherProcessor applicationPatcherProcessor) {
			this.applicationPatcherSelfConfiguration = applicationPatcherSelfConfiguration;
			this.applicationPatcherProcessor = applicationPatcherProcessor;
			log = Log.For(this);
		}

		[DoNotAddLogOffset]
		public void PatchSelfApplication() {
			log.Info("Patching all mono cecil applications...");

			var monoCecilApplicationResultNames = ShiftMonoCecilApplications(applicationPatcherSelfConfiguration.MonoCecilApplicationNames, applicationPatcherSelfConfiguration.MonoCecilResultDirectoryName, false);

			foreach (var monoCecilResultApplicationName in monoCecilApplicationResultNames) {
				log.Info($"Patching '{monoCecilResultApplicationName}' application...");
				PatchApplication(applicationPatcherProcessor, monoCecilResultApplicationName);
				log.Info($"Application '{monoCecilResultApplicationName}' was patched");
			}

			log.Info("All mono cecil applications was patched");
			ShiftMonoCecilApplications(monoCecilApplicationResultNames, applicationPatcherSelfConfiguration.ApplicationPatcherCoreDirectoryName, true);
		}

		[DoNotAddLogOffset]
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

		private static void PatchApplication(ApplicationPatcherProcessor applicationPatcherProcessor, string applicationPath) {
			applicationPatcherProcessor.PatchApplication(applicationPath);
		}
	}
}