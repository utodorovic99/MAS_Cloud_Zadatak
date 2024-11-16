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
		/// Checks whether book title exists.
		/// </summary>
		/// <param name="titleName">Title name to check.</param>
		/// <returns><c>True</c> if <paramref name="titleName"/> exists in the bookstore; otherwise returns <c>false</c>.</returns>
		Task<bool> Exists(string titleName);
	}
}