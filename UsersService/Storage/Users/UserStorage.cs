using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using StorageManagement;
using System;
using System.IO;
using System.Threading.Tasks;
using UsersServiceContract.Enums;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Class representing storage of users.
	/// </summary>
	internal sealed class UserStorage : EntityStorage<string, UserStorageModel>, IUserStorage
	{
		private readonly string enlistedMoneyTranstersStorageKey;
		private IReliableDictionary<uint, MoneyTransferStorageModel> inProgressMoneyTransfers;

		/// <summary>
		/// Initializes new instance of <see cref="TitleStorage"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		public UserStorage(IReliableStateManager stateManager)
			: base(stateManager, repositoryStorageKey: "UserStorage", storageFileFullName: GetStorageFileFullName())
		{
			enlistedMoneyTranstersStorageKey = "MoneyTransfersInProgress";
		}

		/// <inheritdoc/>
		public async Task<bool> Exists(string username)
		{
			bool userExists = false;

			using (var tx = stateManager.CreateTransaction())
			{
				userExists = await entityRepository.ContainsKeyAsync(tx, username);

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return userExists;
		}

		/// <inheritdoc/>
		public async Task<EnlistMoneyTransferResult> EnlistMoneyTransfer(string userId, float amount)
		{
			EnlistMoneyTransferResult enlistmentResult;

			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					enlistmentResult = await EnlistMoneyTransferInternal(tx, userId, amount);
					await tx.CommitAsync();
					return enlistmentResult;
				}
				catch
				{
					//Do not commit and let the azure fabric infrastructure to rollback transaction automatically.
				}
			}

			return new EnlistMoneyTransferResult()
			{
				Status = EnlistMoneyTransferStatus.Fail,
				TransactionId = 0
			};
		}

		/// <inheritdoc/>
		public async Task<bool> ConfirmEnlistedMoneyTransfer(uint transferId)
		{
			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					await inProgressMoneyTransfers.TryRemoveAsync(tx, transferId);
					await tx.CommitAsync();
					return true;
				}
				catch
				{
					//Do not commit and let the azure fabric infrastructure to rollback transaction automatically.
				}
			}

			return false;
		}

		/// <inheritdoc/>
		public async Task<bool> RevokeEnlistedMoneyTransfer(uint transferId)
		{
			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					ConditionalValue<MoneyTransferStorageModel> revokedTransferFromStorage = await inProgressMoneyTransfers.TryRemoveAsync(tx, transferId);
					if (!revokedTransferFromStorage.HasValue)
					{
						return false;
					}

					MoneyTransferStorageModel revokedTransfer = revokedTransferFromStorage.Value;

					ConditionalValue<UserStorageModel> userInStorage = await entityRepository.TryGetValueAsync(tx, revokedTransfer.Username);
					if (!userInStorage.HasValue)
					{
						return false;
					}

					UserStorageModel user = userInStorage.Value;
					user.Saldo += revokedTransfer.Amount;

					await tx.CommitAsync();
					return true;
				}
				catch
				{
					//Do not commit and let the azure fabric infrastructure to rollback transaction automatically.
				}
			}

			return false;
		}

		/// <inheritdoc/>
		protected override string GetEntityKey(UserStorageModel entity)
		{
			return entity.Username;
		}

		/// <inheritdoc/>
		protected override IStorageLoader<UserStorageModel> CreateStorageFileLoader()
		{
			return new JSONStorageLoader<UserStorageModel>(storageFileFullName);
		}

		/// <inheritdoc/>
		protected async override void InitializeStorageAsync()
		{
			base.InitializeStorageAsync();
			inProgressMoneyTransfers = await stateManager.GetOrAddAsync<IReliableDictionary<uint, MoneyTransferStorageModel>>(enlistedMoneyTranstersStorageKey);
		}

		/// <summary>
		/// Internal logic for enlisting money transfer from user account with <paramref name="userId"/> for <paramref name="amount"/>.
		/// </summary>
		/// <param name="transaction">Transaction under which operation is being performed.</param>
		/// <param name="userId">Unique user identifier whose money will be transfered.</param>
		/// <param name="amount">Amount of money to transfer.</param>
		/// <returns>Result indicating result of money transfer enlistment.</returns>
		private async Task<EnlistMoneyTransferResult> EnlistMoneyTransferInternal(ITransaction transaction, string userId, float amount)
		{
			ConditionalValue<UserStorageModel> userInStorage = await entityRepository.TryGetValueAsync(transaction, userId);
			if (!userInStorage.HasValue)
			{
				return new EnlistMoneyTransferResult()
				{
					Status = EnlistMoneyTransferStatus.Fail,
					TransactionId = 0,
				};
			}

			UserStorageModel user = userInStorage.Value;
			if (user.Saldo < amount)
			{
				return new EnlistMoneyTransferResult()
				{
					Status = EnlistMoneyTransferStatus.InsuffientFunds,
					TransactionId = 0,
				};
			}

			user.Saldo -= amount;
			uint transferId = TransferIdGenerator.Generate();
			MoneyTransferStorageModel moneyTransfer = new MoneyTransferStorageModel()
			{
				TransferId = transferId,
				Username = userId,
				Amount = amount,
			};

			await inProgressMoneyTransfers.AddAsync(transaction, moneyTransfer.TransferId, moneyTransfer);

			return new EnlistMoneyTransferResult()
			{
				Status = EnlistMoneyTransferStatus.Success,
				TransactionId = transferId,
			};
		}

		/// <summary>
		/// Gets storage file full name.
		/// </summary>
		/// <returns>Storage file full name.</returns>
		private static string GetStorageFileFullName()
		{
			return Path.Combine(Environment.CurrentDirectory, $"PackageRoot\\AdditionalFiles\\UsersDBFake.json");
		}
	}
}
