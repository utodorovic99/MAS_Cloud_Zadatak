namespace ValidationDataModel
{
	/// <summary>
	/// Validation response for purchase requests.
	/// </summary>
	public class PurchaseValidationResponse
	{
		/// <summary>
		/// Gets or sets purchase validation status.
		/// </summary>
		public PurchaseValidityStatus ValidityStatus { get; set; }
	}
}
