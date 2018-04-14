using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ApplicationPatcher.Core.Factories;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Patchers;
using ApplicationPatcher.Core.Types.Common;

namespace ApplicationPatcher.Core {
	public class ApplicationPatcherProcessor {
		private static readonly string[] availableExtensions = { ".exe", ".dll" };
		private readonly CommonAssemblyFactory commonAssemblyFactory;
		private readonly ILoadedAssemblyPatcher[] loadedAssemblyPatchers;
		private readonly INotLoadedAssemblyPatcher[] notLoadedAssemblyPatchers;
		private readonly Log log;

		public ApplicationPatcherProcessor(CommonAssemblyFactory commonAssemblyFactory,
										   ILoadedAssemblyPatcher[] loadedAssemblyPatchers,
										   INotLoadedAssemblyPatcher[] notLoadedAssemblyPatchers) {
			this.commonAssemblyFactory = commonAssemblyFactory;
			this.loadedAssemblyPatchers = loadedAssemblyPatchers;
			this.notLoadedAssemblyPatchers = notLoadedAssemblyPatchers;
			log = Log.For(this);
		}

		[DoNotAddLogOffset]
		public void PatchApplication(string applicationPath) {
			ResetCurrentDirectory();
			CheckApplicationPath(applicationPath);
			SetCurrentDirectory(applicationPath);

			var applicationFullPath = Path.GetFullPath(Path.GetFileName(applicationPath) ?? throw new Exception());

			log.Info("Reading assembly...");
			var assembly = commonAssemblyFactory.Create(applicationFullPath);
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
			commonAssemblyFactory.Save(assembly, applicationFullPath);
			log.Info("Assembly was saved");
		}

		[DoNotAddLogOffset]
		private static void ResetCurrentDirectory() {
			Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception());
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

			log.Info($"Application was found: {applicationFullPath}");
		}

		[DoNotAddLogOffset]
		private void SetCurrentDirectory(string applicationPath) {
			var currentDirectory = Path.GetDirectoryName(Path.GetFullPath(applicationPath));
			log.Info($"Current directory: {currentDirectory}");

			Directory.SetCurrentDirectory(currentDirectory ?? throw new Exception());
		}

		[DoNotAddLogOffset]
		private PatchResult PatchApplication(IEnumerable<IPatcher> patchers, CommonAssembly assembly) {
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