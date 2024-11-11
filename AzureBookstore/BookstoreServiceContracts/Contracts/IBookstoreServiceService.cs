using BookstoreServiceContracts.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreServiceContracts.Contracts
{
	/// <summary>
	/// Contracts for operations on bookstore service.
	/// </summary>
	public interface IBookstoreServiceService
	{
		/// <summary>
		/// Gets collection of all titles known by the bookstore.
		/// </summary>
		/// <returns>Collection of all titles known by the bookstore.</returns>
		Task<IEnumerable<BookstoreTitle>> GetAllTitles();
	}
}
