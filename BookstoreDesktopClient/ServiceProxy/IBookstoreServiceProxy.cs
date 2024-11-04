using BookstoreDataModel;

namespace BookstoreDesktopClient.ServiceProxy
{
	/// <summary>
	/// Interface for operations related to bookstore service operations.
	/// </summary>
	internal interface IBookstoreServiceProxy : IDisposable
	{
		/// <summary>
		/// Event occurring when response for purchase message is received.
		/// </summary>
		event PurchaseResponseReceived PurchaseReceivedEvent;

		/// <summary>
		/// Sends purchase request and handles is asynchronously.
		/// </summary>
		/// <param name="purchaseRequest">Purchase request to send.</param>
		void SendPurchaseRequest(PurchaseRequest purchaseRequest);
	}
}