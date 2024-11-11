using BookstoreDesktopClient.Resources;

namespace BookstoreDesktopClient.Localization
{
	/// <summary>
	/// Class hosting shared instance of <see cref="BookstoreResources"/> used for localization.
	/// </summary>
	public class LocalizedStrings
	{
		/// <summary>
		/// Gets shared instance of <see cref="BookstoreResources"/>,
		/// </summary>
		public static BookstoreResources Resources { get; } = new BookstoreResources();
	}
}
