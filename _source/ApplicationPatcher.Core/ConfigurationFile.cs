using System;
using System.IO;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core {
	public abstract class ConfigurationFile<TConfiguration> where TConfiguration : ConfigurationFile<TConfiguration>, new() {
		protected abstract string ConfigurationFileName { get; }
		private string configurationFileDirectory;

		[UsedImplicitly, DoNotAddLogOffset]
		public static TConfiguration ReadConfiguration(string containingDirectory = null) {
			containingDirectory = containingDirectory ?? string.Empty;
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception();
			var configurationFileName = Path.Combine(containingDirectory, new TConfiguration().ConfigurationFileName);

			var configurationFileDirectory = currentDirectory;
			while (!File.Exists(Path.Combine(configurationFileDirectory, configurationFileName)))
				if (string.IsNullOrEmpty(configurationFileDirectory = Path.GetDirectoryName(configurationFileDirectory)))
					return new TConfiguration { configurationFileDirectory = Path.Combine(currentDirectory, containingDirectory) };

			try {
				var configuration = XmlSerializerHelper.Deserializing<TConfiguration>(File.ReadAllBytes(Path.Combine(configurationFileDirectory, configurationFileName)));
				configuration.configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory);
				return configuration;
			}
			catch (Exception) {
				return new TConfiguration { configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory) };
			}
		}

		[UsedImplicitly, DoNotAddLogOffset]
		public void WriteConfiguration() {
			if (!Directory.Exists(configurationFileDirectory))
				Directory.CreateDirectory(configurationFileDirectory);

			File.WriteAllBytes(Path.Combine(configurationFileDirectory, ConfigurationFileName), XmlSerializerHelper.Serializing(this));
		}
	}
}