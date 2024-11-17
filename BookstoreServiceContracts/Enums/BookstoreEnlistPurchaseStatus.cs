namespace BookstoreServiceContract.Enums
{
	/// <summary>
	/// Enumeration for book purchase enlistment result.
	/// </summary>
	public enum BookstoreEnlistPurchaseStatus
	{
		/// <summary>
		/// Internal failure.
		/// </summary>
		Fail = -1,

		/// <summary>
		/// Enlistment succeeded.
		/// </summary>
		Success = 0,

		/// <summary>
		/// Book title exists but currently sold-out.
		/// </summary>
		OutOfCopies = 1,
	}
}
