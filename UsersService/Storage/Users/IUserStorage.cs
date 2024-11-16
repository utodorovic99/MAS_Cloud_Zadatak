using StorageManagement;
using System.Threading.Tasks;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Contract for user storage.
	/// </summary>
	internal interface IUserStorage : IStorage
	{
		/// <summary>
		/// Checks whether <paramref name="username"/> exists in storage.
		/// </summary>
		/// <param name="username">Username to check.</param>
		/// <returns><c>True</c> if username exists in storage; otherwise returns <c>false</c>.</returns>
		Task<bool> Exists(string username);
	}
}
