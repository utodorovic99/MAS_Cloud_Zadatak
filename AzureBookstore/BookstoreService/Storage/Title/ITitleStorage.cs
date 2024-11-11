using BookstoreServiceContracts.Model;
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
	}
}