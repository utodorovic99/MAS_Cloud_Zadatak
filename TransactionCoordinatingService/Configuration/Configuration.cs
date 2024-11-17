using System;
using System.Fabric;

namespace TransactionCoordinatingService
{
	/// <summary>
	/// Class encapsulating data from Settings.xml
	/// </summary>
	internal sealed class Configuration
	{
		/// <summary>
		/// Gets URI of bookstore service.
		/// </summary>
		public Uri BookstoreServiceUri { get; private set; }

		/// <summary>
		/// Gets URI of users service.
		/// </summary>
		public Uri UsersServiceUri { get; private set; }

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
					case "BookstoreService":
						BookstoreServiceUri = new Uri(parameter.Value);
						break;

					case "UsersService":
						UsersServiceUri = new Uri(parameter.Value);
						break;
				}
			}
		}
	}
}
