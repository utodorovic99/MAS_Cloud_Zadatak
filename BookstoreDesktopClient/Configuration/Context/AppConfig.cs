namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Class representing application configuration.
	/// </summary>
	public sealed class AppConfig
	{
		/// <summary>
		/// Gets or sets user configuration.
		/// </summary>
		public UserConfig UserConfig { get; set; }

		/// <summary>
		/// Gets or sets configuration for bookstore service.
		/// </summary>
		public BookStoreServiceConfig BookStoreServiceConfig { get; set; }
	}
}
