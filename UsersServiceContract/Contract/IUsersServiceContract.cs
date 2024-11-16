using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace UsersServiceContract.Contract
{
	/// <summary>
	/// Contracts for operations on users service.
	/// </summary>
	public interface IUsersServiceContract : IService
	{
		/// <summary>
		/// Checks whether <paramref name="username"/> exists in storage.
		/// </summary>
		/// <param name="username">Username to check.</param>
		/// <returns><c>True</c> if username exists in storage; otherwise returns <c>false</c>.</returns>
		Task<bool> CheckUsernameExists(string username);
	}
}
