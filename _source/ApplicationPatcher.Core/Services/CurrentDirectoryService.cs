using System;
using System.IO;
using System.Reflection;

namespace ApplicationPatcher.Core.Services {
	public class CurrentDirectoryService : IDisposable {
		public static CurrentDirectoryService FromExecutingAssembly() {
			return From(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
		}

		public static CurrentDirectoryService From(string newCurrentDirectory) {
			return new CurrentDirectoryService(newCurrentDirectory);
		}

		private readonly string oldCurrentDirectory;

		private CurrentDirectoryService(string newCurrentDirectory) {
			oldCurrentDirectory = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(newCurrentDirectory);
		}

		public void Dispose() {
			Directory.SetCurrentDirectory(oldCurrentDirectory);
		}
	}
}