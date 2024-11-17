using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;
using UsersServiceContract.Enums;

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

		/// <summary>
		/// Enlists money transfer from user account with <paramref name="userId"/> for <paramref name="amount"/>.
		/// </summary>
		/// <param name="userId">Unique user identifier whose money will be transfered.</param>
		/// <param name="amount">Amount of money to transfer.</param>
		/// <returns>Result indicating result of money transfer enlistment.</returns>
		Task<EnlistMoneyTransferResult> EnlistMoneyTransfer(string userId, float amount);

		/// <summary>
		/// Confirms enlisted money transfer.
		/// </summary>
		/// <param name="transferId">Transfer id to confirm.</param>
		/// <returns><c>True</c> if confirmation succeeded; otherwise returns <c>false</c>.</returns>
		Task<bool> ConfirmEnlistedMoneyTransfer(uint transferId);

		/// <summary>
		/// Revokes previously enlisted money transfer.
		/// </summary>
		/// <param name="transferId">Transfer id to revoke.</param>
		/// <returns><c>True</c> if revoking succeeded; otherwise returns <c>false</c>.</returns>
		Task<bool> RevokeEnlistedMoneyTransfer(uint transferId);
	}
}
