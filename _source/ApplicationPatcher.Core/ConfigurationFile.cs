using System;
using System.IO;
using System.Reflection;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core {
	public abstract class ConfigurationFile<TConfiguration> where TConfiguration : ConfigurationFile<TConfiguration>, new() {
		protected abstract string ConfigurationFileName { get; }
		private string configurationFileDirectory;

		[UsedImplicitly]
		public static TConfiguration ReadConfiguration(string containingDirectory = null) {
			containingDirectory = containingDirectory ?? string.Empty;
			var configurationFileName = Path.Combine(containingDirectory, new TConfiguration().ConfigurationFileName);

			var configurationFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			while (!string.IsNullOrEmpty(configurationFileDirectory) && !File.Exists(Path.Combine(configurationFileDirectory, configurationFileName)))
				configurationFileDirectory = Path.GetDirectoryName(configurationFileDirectory);

			if (string.IsNullOrEmpty(configurationFileDirectory))
				return new TConfiguration { configurationFileDirectory = containingDirectory };

			try {
				var configuration = XmlSerializerHelper.Deserializing<TConfiguration>(File.ReadAllBytes(Path.Combine(configurationFileDirectory, configurationFileName)));
				configuration.configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory);
				return configuration;
			}
			catch (Exception) {
				return new TConfiguration { configurationFileDirectory = containingDirectory };
			}
		}

		[UsedImplicitly]
		public void WriteConfiguration() {
			if (!string.IsNullOrEmpty(configurationFileDirectory) && !Directory.Exists(configurationFileDirectory))
				Directory.CreateDirectory(configurationFileDirectory);

			File.WriteAllBytes(Path.Combine(configurationFileDirectory ?? string.Empty, ConfigurationFileName), XmlSerializerHelper.Serializing(this));
		}
	}
}