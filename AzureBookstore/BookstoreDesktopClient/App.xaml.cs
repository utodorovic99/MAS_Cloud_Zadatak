using BookstoreDesktopClient.Configuration;
using System.Windows;

namespace BookstoreDesktopClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Initializes new instance of <see cref="App"/>
		/// </summary>
		static App()
		{
			AppConfigLoader configLoader = new AppConfigLoader();
			Configuration = configLoader.Load();
		}

		/// <summary>
		/// Shared application configuration.
		/// </summary>
		public static AppConfig Configuration { get; private set; }
	}
}
