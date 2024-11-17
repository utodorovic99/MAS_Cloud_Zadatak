using BookstoreServiceContract.Model;
using Common.Model;
using StorageManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Contract for title storage.
	/// </summary>
	internal interface ITitleStorage : IStorage
	{
		/// <summary>
		/// Gets all titles known by the bookstore.
		/// </summary>
		/// <returns>All titles known by the bookstore.</returns>
		Task<IEnumerable<BookstoreTitle>> GetAllTitles();

		/// <summary>
		/// Gets title with <paramref name="titleName"/>.
		/// </summary>
		/// <returns>Title with nae <paramref name="titleName"/>.</returns>
		Task<BookstoreTitle> GetTitle(string titleName);

		/// <summary>
		/// Checks whether book title exists.
		/// </summary>
		/// <param name="titleName">Title name to check.</param>
		/// <returns><c>True</c> if <paramref name="titleName"/> exists in the bookstore; otherwise returns <c>false</c>.</returns>
		Task<bool> Exists(string titleName);

		/// <summary>
		/// Enlists copy of <paramref name="bookId"/> for purchasing.
		/// </summary>
		/// <param name="bookId">Book identifier (title name).</param>
		/// <returns>Bookstore purchase enlistment result.</returns>
		Task<BookstoreEnlistPurchaseResult> EnlistBookForPurchase(string bookId);

		/// <summary>
		/// Confirms enlisted purchase.
		/// </summary>
		/// <param name="purchaseId">Purchase id to confirm.</param>
		/// <returns><c>True</c> if confirmation succeeded; otherwise returns <c>false</c>.</returns>
		Task<bool> ConfirmEnlistedPurchase(uint purchaseId);

		/// <summary>
		/// Revokes previously enlisted purchase.
		/// </summary>
		/// <param name="purchaseId">Purchase id to revoke.</param>
		/// <returns><c>True</c> if revoking succeeded; otherwise returns <c>false</c>.</returns>
		Task<bool> RevokeEnlistedPurchase(uint purchaseId);
	}
}