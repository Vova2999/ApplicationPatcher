using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ApplicationPatcher.Core;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;

namespace ApplicationPatcher.Self {
	public class ApplicationPatcherSelfProcessor {
		private const string resultDirectoryName = "Result";
		private static readonly string[] monoCecilApplicationNames = { "Mono.Cecil.dll", "Mono.Cecil.Mdb.dll", "Mono.Cecil.Pdb.dll", "Mono.Cecil.Rocks.dll" };
		private static readonly Dictionary<string, string> monoCecilResultApplicationNames = monoCecilApplicationNames
			.ToDictionary(monoCecilApplicationName => monoCecilApplicationName, monoCecilApplicationName => Path.Combine(resultDirectoryName, monoCecilApplicationName));

		private readonly ApplicationPatcherProcessor applicationPatcherProcessor;
		private readonly Log log;

		public ApplicationPatcherSelfProcessor(ApplicationPatcherProcessor applicationPatcherProcessor) {
			this.applicationPatcherProcessor = applicationPatcherProcessor;
			log = Log.For(this);
		}

		public void PatchSelfApplication() {
			ShiftMonoCecilApplication();

			foreach (var monoCecilResultApplicationName in monoCecilResultApplicationNames.Values) {
				log.Info($"Patch '{monoCecilResultApplicationName}'");
				applicationPatcherProcessor.PatchApplication(monoCecilResultApplicationName);
			}
		}

		private void ShiftMonoCecilApplication() {
			log.Info($"Shift mono cecil application to '{resultDirectoryName}' directory...");

			if (Directory.Exists(resultDirectoryName)) {
				log.Debug($"Delete '{resultDirectoryName}' directory");
				Directory.Delete(resultDirectoryName, true);
				Thread.Sleep(500);
			}

			log.Debug($"Create '{resultDirectoryName}' directory");
			Directory.CreateDirectory(resultDirectoryName);
			monoCecilApplicationNames.ForEach(monoCecilApplicationName => File.Copy(monoCecilApplicationName, monoCecilResultApplicationNames[monoCecilApplicationName]));

			log.Info("Mono cecil application was shifted");
		}
	}
}