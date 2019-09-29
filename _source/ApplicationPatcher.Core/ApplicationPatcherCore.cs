using System;
using System.IO;
using System.Linq;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Factories;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Logs;
using ApplicationPatcher.Core.Patchers;

namespace ApplicationPatcher.Core {
	public class ApplicationPatcherCore {
		private static readonly string[] AvailableExtensions = { ".exe", ".dll" };

		private readonly ICommonAssemblyFactory commonAssemblyFactory;
		private readonly PatcherOnLoadedApplication[] patchersOnLoadedApplication;
		private readonly PatcherOnPatchedApplication[] patchersOnPatchedApplication;
		private readonly PatcherOnNotLoadedApplication[] patchersOnNotLoadedApplication;
		private readonly ILog log;

		public ApplicationPatcherCore(ICommonAssemblyFactory commonAssemblyFactory,
									  PatcherOnLoadedApplication[] patchersOnLoadedApplication,
									  PatcherOnPatchedApplication[] patchersOnPatchedApplication,
									  PatcherOnNotLoadedApplication[] patchersOnNotLoadedApplication) {
			this.commonAssemblyFactory = commonAssemblyFactory;
			this.patchersOnLoadedApplication = patchersOnLoadedApplication;
			this.patchersOnPatchedApplication = patchersOnPatchedApplication;
			this.patchersOnNotLoadedApplication = patchersOnNotLoadedApplication;
			log = Log.For(this);
		}

		public void PatchApplication(string applicationPath, string signaturePath = null) {
			CheckApplicationPath(applicationPath);

			log.Info("Reading assembly...");
			var assembly = commonAssemblyFactory.Create(applicationPath);
			log.Info("Assembly was readed");

			if (PatchHelper.PatchApplication(patchersOnNotLoadedApplication, patcher => patcher.Patch(assembly), log) == PatchResult.Cancel)
				return;

			log.Info("Loading assembly...");
			assembly.Load();
			log.Info("Assembly was loaded");

			if (assembly.TypesFromThisAssembly.Any())
				log.Debug("Types from this assembly found:", assembly.TypesFromThisAssembly.Select(type => type.FullName).OrderBy(fullName => fullName));
			else
				log.Debug("Types from this assembly not found");

			if (PatchHelper.PatchApplication(patchersOnLoadedApplication, patcher => patcher.Patch(assembly), log) == PatchResult.Cancel)
				return;

			if (PatchHelper.PatchApplication(patchersOnPatchedApplication, patcher => patcher.Patch(assembly), log) == PatchResult.Cancel)
				return;

			log.Info("Application was patched");

			log.Info("Save assembly...");
			commonAssemblyFactory.Save(assembly, applicationPath, signaturePath);
			log.Info("Assembly was saved");
		}

		private void CheckApplicationPath(string applicationPath) {
			if (applicationPath.IsNullOrEmpty())
				throw new ArgumentException("You must specify path to application");

			var applicationFullPath = Path.GetFullPath(applicationPath);
			if (!File.Exists(applicationFullPath))
				throw new FileNotFoundException($"Not found application: {applicationFullPath}");

			var applicationExtension = Path.GetExtension(applicationPath);
			if (!AvailableExtensions.Any(availableExtension => string.Equals(availableExtension, applicationExtension, StringComparison.OrdinalIgnoreCase)))
				throw new FileLoadException($"Extension of application can not be '{applicationExtension}'. " +
					$"Available extensions: {AvailableExtensions.Select(availableExtension => $"'{availableExtension}'").JoinToString(", ")}");

			log.Info($"Application was found: '{applicationFullPath}'");
		}
	}
}