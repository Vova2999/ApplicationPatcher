using System;
using System.IO;
using System.Reflection;
using ApplicationPatcher.Core.Extensions;
using ApplicationPatcher.Core.Helpers;
using ApplicationPatcher.Core.Logs;
using JetBrains.Annotations;

namespace ApplicationPatcher.Core {
	[PublicAPI]
	public abstract class ConfigurationFile<TConfiguration> where TConfiguration : ConfigurationFile<TConfiguration>, new() {
		private static readonly ILog Log = Logs.Log.For(typeof(ConfigurationFile<TConfiguration>));

		protected abstract string ConfigurationFileName { get; }
		private string configurationFileDirectory;

		[NotNull]
		public static TConfiguration ReadConfiguration(string containingDirectory = null) {
			var configuration = new TConfiguration();
			containingDirectory = containingDirectory ?? string.Empty;

			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception();
			var configurationFileName = Path.Combine(containingDirectory, configuration.ConfigurationFileName);

			var configurationFileDirectory = currentDirectory;
			while (configurationFileDirectory.IsSignificant() && !File.Exists(Path.Combine(configurationFileDirectory, configurationFileName)))
				configurationFileDirectory = Path.GetDirectoryName(configurationFileDirectory);

			if (configurationFileDirectory.IsNullOrEmpty()) {
				configuration.configurationFileDirectory = Path.Combine(currentDirectory, containingDirectory);
				return configuration;
			}

			try {
				var configurationContent = File.ReadAllBytes(Path.Combine(configurationFileDirectory, configurationFileName));
				configuration = XmlSerializerHelper.Deserializing<TConfiguration>(configurationContent);
				configuration.configurationFileDirectory = Path.Combine(configurationFileDirectory, containingDirectory);
				return configuration;
			}
			catch (Exception) {
				Log.Warn($"Error on read configuration file {configuration.ConfigurationFileName}");
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