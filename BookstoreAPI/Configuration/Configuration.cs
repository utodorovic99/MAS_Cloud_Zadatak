﻿using System.Fabric;

namespace BookstoreAPI
{
	/// <summary>
	/// Class encapsulating data from Settings.xml
	/// </summary>
	internal sealed class Configuration
	{
		/// <summary>
		/// Gets URI of validation service.
		/// </summary>
		public Uri ValidationServiceUri { get; private set; }

		/// <summary>
		/// Initializes self from Settings.xml.
		/// </summary>
		public void Initialize()
		{
			CodePackageActivationContext context = FabricRuntime.GetActivationContext();
			var configSettings = context.GetConfigurationPackageObject("Config").Settings;
			var data = configSettings.Sections["ServiceAddresses"];
			foreach (var parameter in data.Parameters)
			{
				switch (parameter.Name)
				{
					case "ValidationService":
						ValidationServiceUri = new Uri(parameter.Value);
						break;
				}
			}
		}
	}
}