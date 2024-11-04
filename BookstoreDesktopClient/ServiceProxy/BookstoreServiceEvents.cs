using BookstoreDesktopClient.ViewModel;

namespace BookstoreDesktopClient.ServiceProxy
{
	/// <summary>
	/// Event occurring when response for purchase message is received.
	/// </summary>
	/// <param name="purchaseResponse">Purchase response.</param>
	public delegate void PurchaseResponseReceived(PurchaseResponseWrapper purchaseResponse);
}
