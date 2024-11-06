namespace PurchaseDataModel
{
	/// <summary>
	/// Enumeration for title purchase response status.
	/// </summary>
	public enum PurchaseResponseStatus : short
	{
		/// <summary>
		/// Internal failure.
		/// </summary>
		Fail = -1,

		/// <summary>
		/// Purchase succeeded.
		/// </summary>
		Success = 0,

		/// <summary>
		/// Invalid username.
		/// </summary>
		InvalidUser = 1,

		/// <summary>
		/// User has not enough funds.
		/// </summary>
		InsufficientFunds = 2,

		/// <summary>
		/// Book title does not exists.
		/// </summary>
		UnknownBookTitle = 3,

		/// <summary>
		/// Book title exists but currently sold-out.
		/// </summary>
		OutOfCopies = 4,
	}
}
