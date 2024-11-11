namespace ValidationDataModel
{
	/// <summary>
	/// Enumeration for title purchase validity status.
	/// </summary>
	public enum PurchaseValidityStatus : short
	{
		/// <summary>
		/// General purpose indicator that request is invalid.
		/// </summary>
		Invalid = 0,

		/// <summary>
		/// Purchase request is valid.
		/// </summary>
		Valid = 1,

		/// <summary>
		/// Invalid username.
		/// </summary>
		InvalidUser = 2,

		/// <summary>
		/// Book title does not exists.
		/// </summary>
		UnknownBookTitle = 3,
	}
}
