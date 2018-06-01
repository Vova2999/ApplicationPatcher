using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApplicationPatcher.Core.Factories;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core {
	[UsedImplicitly]
	public class ApplicationPatcherProcessor {
		private static readonly string[] availableExtensions = { ".exe", ".dll" };
		private readonly CommonAssemblyFactory commonAssemblyFactory;
		private readonly LoadedAssemblyPatcher[] loadedAssemblyPatchers;
		private readonly NotLoadedAssemblyPatcher[] notLoadedAssemblyPatchers;
		private readonly Log log;

		public ApplicationPatcherProcessor(CommonAssemblyFactory commonAssemblyFactory,
										   LoadedAssemblyPatcher[] loadedAssemblyPatchers,
										   NotLoadedAssemblyPatcher[] notLoadedAssemblyPatchers) {
			this.commonAssemblyFactory = commonAssemblyFactory;
			this.loadedAssemblyPatchers = loadedAssemblyPatchers;
			this.notLoadedAssemblyPatchers = notLoadedAssemblyPatchers;
			log = Log.For(this);
		}

		[DoNotAddLogOffset]
		public void PatchApplication(string applicationPath, string signaturePath = null) {
			CheckApplicationPath(applicationPath);

			log.Info("Reading assembly...");
			var assembly = commonAssemblyFactory.Create(applicationPath);
			log.Info("Assembly was readed");

			var notLoadedAssemblyPatchResult = PatchApplication(notLoadedAssemblyPatchers, assembly);
			if (notLoadedAssemblyPatchResult == PatchResult.Canceled)
				return;

			log.Info("Loading assembly...");
			assembly.Load();
			log.Info("Assembly was loaded");

			if (assembly.TypesFromThisAssembly.Any())
				log.Debug("Types from this assembly found:", assembly.TypesFromThisAssembly.Select(type => type.FullName).OrderBy(fullName => fullName));
			else
				log.Debug("Types from this assembly not found");

			var loadedAssemblyPatchResult = PatchApplication(loadedAssemblyPatchers, assembly);
			if (loadedAssemblyPatchResult == PatchResult.Canceled)
				return;

			log.Info("Application was patched");

			log.Info("Save assembly...");
			commonAssemblyFactory.Save(assembly, applicationPath, signaturePath);
			log.Info("Assembly was saved");
		}

		[DoNotAddLogOffset]
		private void CheckApplicationPath(string applicationPath) {
			if (string.IsNullOrEmpty(applicationPath))
				throw new ArgumentException("You must specify path to application");

			var applicationFullPath = Path.GetFullPath(applicationPath);
			if (!File.Exists(applicationFullPath))
				throw new FileNotFoundException($"Not found application: {applicationFullPath}");

			var applicationExtension = Path.GetExtension(applicationPath);
			if (!availableExtensions.Any(availableExtension => string.Equals(availableExtension, applicationExtension, StringComparison.InvariantCultureIgnoreCase)))
				throw new FileLoadException($"Extension of application can not be '{applicationExtension}'. " +
					$"Available extensions: {string.Join(", ", availableExtensions.Select(availableExtension => $"'{availableExtension}'"))}");

			log.Info($"Application was found: '{applicationFullPath}'");
		}

		[DoNotAddLogOffset]
		private PatchResult PatchApplication(IEnumerable<IPatcher> patchers, CommonAssembly assembly) {
			if (patchers == null)
				return PatchResult.Succeeded;

			foreach (var patcher in patchers) {
				log.Info($"Apply '{patcher.GetType().FullName}' patcher...");
				var patchResult = patcher.Patch(assembly);
				log.Info($"Patcher '{patcher.GetType().FullName}' was applied with result: {patchResult}");

				switch (patchResult) {
					case PatchResult.Succeeded:
						continue;
					case PatchResult.Canceled:
						log.Info("Patching application was canceled");
						return PatchResult.Canceled;
					default:
						throw new ArgumentOutOfRangeException(nameof(patchResult));
				}
			}

			return PatchResult.Succeeded;
		}
	}
}