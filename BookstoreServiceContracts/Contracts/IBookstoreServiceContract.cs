using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookstoreServiceContract.Contracts
{
	/// <summary>
	/// Contracts for operations on bookstore service.
	/// </summary>
	public interface IBookstoreServiceContract : IService
	{
		/// <summary>
		/// Gets collection of all titles known by the bookstore.
		/// </summary>
		/// <returns>Collection of all titles known by the bookstore.</returns>
		Task<IEnumerable<BookstoreTitle>> GetAllTitles();

		/// <summary>
		/// Checks whether book title exists.
		/// </summary>
		/// <param name="titleName">Title name to check.</param>
		/// <returns><c>True</c> if <paramref name="titleName"/> exists in the bookstore; otherwise returns <c>false</c>.</returns>
		Task<bool> CheckTitleExists(string titleName);
	}
}
