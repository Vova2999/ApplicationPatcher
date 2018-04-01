using System;
using System.IO;
using System.Linq;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Factories;
using ApplicationPatcher.Core.Helpers;

namespace ApplicationPatcher.Core {
	public class ApplicationPatcherProcessor {
		private static readonly string[] availableExtensions = { ".exe", ".dll" };
		private readonly CommonAssemblyFactory commonAssemblyFactory;
		private readonly IPatcher[] patchers;
		private readonly Log log;

		public ApplicationPatcherProcessor(CommonAssemblyFactory commonAssemblyFactory, IPatcher[] patchers) {
			this.commonAssemblyFactory = commonAssemblyFactory;
			this.patchers = patchers;
			log = Log.For(this);
		}

		public void PatchApplication(string applicationPath) {
			CheckApplicationPath(applicationPath);
			SetCurrentDirectory(applicationPath);

			var assembly = commonAssemblyFactory.Create(applicationPath);
			patchers.ForEach(patcher => patcher.Patch(assembly));
		}

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

		private void SetCurrentDirectory(string applicationPath) {
			var currentDirectory = Path.GetDirectoryName(Path.GetFullPath(applicationPath));
			log.Info($"Current directory: {currentDirectory}");

			Directory.SetCurrentDirectory(currentDirectory ?? throw new Exception());
		}
	}
}