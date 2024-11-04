namespace BookstoreDataModel
{
	/// <summary>
	/// Model class representing single title purchase request.
	/// </summary>
	public class PurchaseRequest
	{
		/// <summary>
		/// Gets or sets name of user who is purchasing the book.
		/// </summary>
		public string User { get; set; }

		/// <summary>
		/// Gets or sets title which is being purchased.
		/// </summary>
		public string Title { get; set; }
	}
}
