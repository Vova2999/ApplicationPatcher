using System;
using System.IO;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace ApplicationPatcher.Core {
	public abstract class ConfigurationFile<TConfiguration> where TConfiguration : ConfigurationFile<TConfiguration>, new() {
		protected abstract string ConfigurationFileName { get; }
		private string configurationFileDirectory;

		[NotNull]
		public static TConfiguration ReadConfiguration(string containingDirectory = null) {
			containingDirectory = containingDirectory ?? string.Empty;
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception();
			var configurationFileName = Path.Combine(containingDirectory, new TConfiguration().ConfigurationFileName);

			var configurationFileDirectory = currentDirectory;
			while (!File.Exists(Path.Combine(configurationFileDirectory, configurationFileName))) {
				configurationFileDirectory = Path.GetDirectoryName(configurationFileDirectory);
				if (configurationFileDirectory.IsNullOrEmpty())
					return new TConfiguration { configurationFileDirectory = Path.Combine(currentDirectory, containingDirectory) };
			}

			try {
				var configuration = XmlSerializerHelper.Deserializing<TConfiguration>(File.ReadAllBytes(Path.Combine(configurationFileDirectory, configurationFileName)));
				configuration.configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory);
				return configuration;
			}
			catch (Exception) {
				return new TConfiguration { configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory) };
			}
		}

		public void WriteConfiguration() {
			var configurationFilePath = Path.Combine(configurationFileDirectory, ConfigurationFileName);
			var configurationFileCurrentDirectory = Path.GetDirectoryName(configurationFilePath) ?? throw new Exception();

			if (!Directory.Exists(configurationFileCurrentDirectory))
				Directory.CreateDirectory(configurationFileCurrentDirectory);

			File.WriteAllBytes(configurationFilePath, XmlSerializerHelper.Serializing(this));
		}
	}
}