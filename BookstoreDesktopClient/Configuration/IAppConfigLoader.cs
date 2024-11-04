namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Interface for operations related to loading of application configuration.
	/// </summary>
	internal interface IAppConfigLoader
	{
		/// <summary>
		/// Initializes App.xaml into a <see cref="AppConfig"/>.
		/// </summary>
		/// <returns><see cref="AppConfig"/> as top level configuration consisted of multiple context specific configurations.</returns>
		AppConfig Load();
	}
}