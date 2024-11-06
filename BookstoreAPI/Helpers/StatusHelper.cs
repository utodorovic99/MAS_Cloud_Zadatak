using PurchaseDataModel;
using ValidationDataModel;

namespace BookstoreAPI
{
	/// <summary>
	/// Helper for different status indicator throughout the system.
	/// </summary>
	internal static class StatusHelper
	{
		private static readonly Dictionary<PurchaseValidityStatus, PurchaseResponseStatus> validityToResponseStatus;

		/// <summary>
		/// Static initialization of <see cref="StatusHelper"/>.
		/// </summary>
		static StatusHelper()
		{
			validityToResponseStatus = new Dictionary<PurchaseValidityStatus, PurchaseResponseStatus>(4);
			validityToResponseStatus[PurchaseValidityStatus.Invalid] = PurchaseResponseStatus.Fail;
			validityToResponseStatus[PurchaseValidityStatus.Valid] = PurchaseResponseStatus.Success;
			validityToResponseStatus[PurchaseValidityStatus.InvalidUser] = PurchaseResponseStatus.InvalidUser;
			validityToResponseStatus[PurchaseValidityStatus.UnknownBookTitle] = PurchaseResponseStatus.UnknownBookTitle;
		}

		/// <summary>
		/// Converts <paramref name="validityStatus"/> to its corresponding <see cref="PurchaseResponseStatus"/>.
		/// </summary>
		/// <param name="validityStatus">Validity status to convert.</param>
		/// <returns><see cref="PurchaseResponseStatus"/> corresponding to <paramref name="validityStatus"/>.</returns>
		public static PurchaseResponseStatus ToResponseStatus(PurchaseValidityStatus validityStatus) => validityToResponseStatus[validityStatus];
	}
}
