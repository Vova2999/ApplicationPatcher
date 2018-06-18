using System;
using System.IO;
using System.Reflection;

namespace ApplicationPatcher.Core.Helpers {
	public class CurrentDirectoryHelper : IDisposable {
		private readonly string oldCurrentDirectory;

		private CurrentDirectoryHelper(string newCurrentDirectory) {
			oldCurrentDirectory = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(newCurrentDirectory);
		}

		public static CurrentDirectoryHelper FromExecutingAssembly() {
			return From(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
		}

		public static CurrentDirectoryHelper From(string newCurrentDirectory) {
			return new CurrentDirectoryHelper(newCurrentDirectory);
		}

		public void Dispose() {
			Directory.SetCurrentDirectory(oldCurrentDirectory);
		}
	}
}