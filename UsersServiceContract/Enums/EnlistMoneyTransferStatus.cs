namespace UsersServiceContract.Enums
{
	/// <summary>
	/// Enumeration for money transfer enlistment status.
	/// </summary>
	public enum EnlistMoneyTransferStatus
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
		/// User has not enough fund for purchase.
		/// </summary>
		InsuffientFunds = 1,
	}
}
