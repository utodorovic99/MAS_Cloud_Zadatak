namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Class representing user configuration.
	/// </summary>
	public sealed class UserConfig : ConfigBase
	{
		/// <summary>
		/// Gets config identifier used in App.config.
		/// </summary>
		public static string Identifier { get; } = "userConfig";

		/// <summary>
		/// Gets or sets username.
		/// </summary>
		[ConfigProperty(Identifier = "username", DataType = typeof(string))]
		public string Username { get; set; }
	}
}
