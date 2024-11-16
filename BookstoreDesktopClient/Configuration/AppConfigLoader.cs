using System.Collections.Generic;
using System.Configuration;

namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Loader for application configuration.
	/// </summary>
	internal sealed class AppConfigLoader : IAppConfigLoader
	{
		/// <inheritdoc/>
		public AppConfig Load()
		{
			CreateEmptyTopLevelConfiguration(out AppConfig topLevelConfiguration);
			CreateInnerConfigurationByIdMap(topLevelConfiguration, out Dictionary<string, ConfigBase> innerConfigsByIdentifier);
			LoadConfigurationsFromFile(innerConfigsByIdentifier);

			return topLevelConfiguration;
		}

		/// <summary>
		/// Creates empty <see cref="AppConfig"/> as top level configuration.
		/// This configuration is consisted of multiple inner, context specific configurations.
		/// </summary>
		/// <param name="topLevelConfiguration">Empty top level configuration.</param>
		private void CreateEmptyTopLevelConfiguration(out AppConfig topLevelConfiguration)
		{
			topLevelConfiguration = new AppConfig();

			topLevelConfiguration.UserConfig = new UserConfig();
			topLevelConfiguration.BookStoreServiceConfig = new BookStoreServiceConfig();
		}

		/// <summary>
		/// Creates mapping from inner configuration identifier (present in App.config) to its corresponding configuration object.
		/// </summary>
		/// <param name="appConfig">Application configuration composed of multiple inner, context specific configurations.</param>
		/// <param name="configByIdentifierMap">Mapping from identifier to its inner configuration object.</param>
		private void CreateInnerConfigurationByIdMap(AppConfig appConfig, out Dictionary<string, ConfigBase> configByIdentifierMap)
		{
			configByIdentifierMap = new Dictionary<string, ConfigBase>(2);
			configByIdentifierMap[UserConfig.Identifier] = appConfig.UserConfig;
			configByIdentifierMap[BookStoreServiceConfig.Identifier] = appConfig.BookStoreServiceConfig;
		}

		/// <summary>
		/// Loads configuration from App.config into their corresponding configuration objects.
		/// </summary>
		/// <param name="configByIdentifierMap">Mapping from configuration identifier to corresponding configuration object.</param>
		private void LoadConfigurationsFromFile(Dictionary<string, ConfigBase> configByIdentifierMap)
		{
			foreach (var configKey in ConfigurationManager.AppSettings.AllKeys)
			{
				if (configKey is null)
				{
					continue;
				}

				string[] configKeyParts = configKey.Split('.');
				if (configKeyParts.Length < 2)
				{
					continue;
				}

				string configClassIdentifier = configKeyParts[0];
				string configPropertyIdentifier = configKeyParts[1];

				if (!configByIdentifierMap.TryGetValue(configClassIdentifier, out ConfigBase config))
				{
					continue;
				}

				string propertyValue = ConfigurationManager.AppSettings[configKey];
				config.SetProperty(configPropertyIdentifier, propertyValue);
			}
		}
	}
}
