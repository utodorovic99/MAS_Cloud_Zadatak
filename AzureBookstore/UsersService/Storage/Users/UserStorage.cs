using Microsoft.ServiceFabric.Data;
using StorageManagement;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UsersService.Storage.Users
{
	/// <summary>
	/// Class representing storage of users.
	/// </summary>
	internal sealed class UserStorage : EntityStorage<string, UserStorageModel>, IUserStorage
	{
		/// <summary>
		/// Initializes new instance of <see cref="TitleStorage"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		public UserStorage(IReliableStateManager stateManager)
			: base(stateManager, repositoryStorageKey: "UserStorage", storageFileFullName: GetStorageFileFullName())
		{
		}

		/// <inheritdoc/>
		public async Task<bool> CheckUsernameExists(string username)
		{
			ConditionalValue<UserStorageModel> user;

			using (var tx = stateManager.CreateTransaction())
			{
				user = await entityRepository.TryGetValueAsync(tx, username);

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return user.HasValue;
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
