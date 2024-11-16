using System;

namespace BookstoreDesktopClient.Configuration
{
	/// <summary>
	/// Class representing configuration for bookstore service.
	/// </summary>
	public sealed class BookStoreServiceConfig : ConfigBase
	{
		/// <summary>
		/// Gets config identifier used in App.config.
		/// </summary>
		public static string Identifier { get; } = "bookstoreServiceConfig";

		/// <summary>
		/// Gets or sets service URL.
		/// </summary>
		[ConfigProperty(Identifier = "Uri", DataType = typeof(Uri))]
		public Uri Uri { get; set; }
	}
}
