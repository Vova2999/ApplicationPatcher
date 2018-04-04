using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

		[DoNotAddLogOffset]
		public void PatchSelfApplication() {
			ResetCurrentDirectory();
			ShiftMonoCecilApplication();

			log.Info("Patching all mono cecil applications...");

			foreach (var monoCecilResultApplicationName in monoCecilResultApplicationNames.Values) {
				log.Info($"Patching '{monoCecilResultApplicationName}' application...");
				applicationPatcherProcessor.PatchApplication(monoCecilResultApplicationName);
				log.Info($"Application '{monoCecilResultApplicationName}' was patched");
			}

			log.Info("All mono cecil applications was patched");
		}

		[DoNotAddLogOffset]
		private static void ResetCurrentDirectory() {
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception());
		}

		[DoNotAddLogOffset]
		private void ShiftMonoCecilApplication() {
			log.Info($"Shifting mono cecil application to '{resultDirectoryName}' directory...");

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