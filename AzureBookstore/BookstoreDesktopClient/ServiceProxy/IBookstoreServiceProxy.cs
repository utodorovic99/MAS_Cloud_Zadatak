using BookstoreServiceContracts.Model;
using PurchaseDataModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

		/// <summary>
		/// Send request which retrieves all known bookstore titles.
		/// </summary>
		/// <returns>Task resulting with set of all known bookstore titles.</returns>
		/// <remarks>
		/// Routine is automatically canceled after predefined period of time.
		/// </remarks>
		Task<IEnumerable<BookstoreTitle>> GetAllTitles();

		/// <summary>
		/// Send request which retrieves all known bookstore titles.
		/// </summary>
		/// <param name="ct">Cancellation token.</param>
		/// <returns>Task resulting with set of all known bookstore titles.</returns>
		Task<IEnumerable<BookstoreTitle>> GetAllTitles(CancellationToken ct);
	}
}