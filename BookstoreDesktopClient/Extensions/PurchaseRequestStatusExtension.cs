using BookstoreDesktopClient.Resources;
using PurchaseDataModel;

namespace BookstoreDesktopClient
{
	/// <summary>
	/// Extension class for <see cref="PurchaseResponseStatus"/>
	/// </summary>
	internal static class PurchaseRequestStatusExtension
	{
		private static readonly Dictionary<PurchaseResponseStatus, string> responseStatusToLocalizedString;

		/// <summary>
		/// Static initialization of <see cref="PurchaseRequestStatusExtension"/>.
		/// </summary>
		static PurchaseRequestStatusExtension()
		{
			responseStatusToLocalizedString = new Dictionary<PurchaseResponseStatus, string>(6);
			responseStatusToLocalizedString[PurchaseResponseStatus.Fail] = BookstoreResources.BookPurschaseResult_REQUEST_FAILED;
			responseStatusToLocalizedString[PurchaseResponseStatus.Success] = BookstoreResources.ResultDialogContent_SUCCESS;
			responseStatusToLocalizedString[PurchaseResponseStatus.InvalidUser] = BookstoreResources.BookPurchaseResult_INVALID_USER;
			responseStatusToLocalizedString[PurchaseResponseStatus.InsufficientFunds] = BookstoreResources.BookPurchaseResult_INSUFFICIENT_FUNDS;
			responseStatusToLocalizedString[PurchaseResponseStatus.UnknownBookTitle] = BookstoreResources.BookPurchaseResult_UNKNOWN_TITLE;
			responseStatusToLocalizedString[PurchaseResponseStatus.OutOfCopies] = BookstoreResources.BookPurchaseResult_SOLD_OUT;
		}

		/// <summary>
		/// Converts <see cref="PurchaseResponseStatus"/> into a localized string.
		/// </summary>
		/// <param name="requestStatus">Request status whose localized string is being requested.</param>
		/// <returns>Localized string for <see cref="PurchaseResponseStatus"/>.</returns>
		public static string ToLocalizedString(this PurchaseResponseStatus requestStatus)
		{
			return responseStatusToLocalizedString[requestStatus];
		}
	}
}
