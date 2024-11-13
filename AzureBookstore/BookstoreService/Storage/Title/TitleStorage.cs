using BookstoreService.Storage.Exceptions;
using BookstoreServiceContract.Model;
using CommunicationsSDK.PlatformExtensions;
using Microsoft.ServiceFabric.Data;
using StorageManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Class representing storage of titles.
	/// </summary>
	internal sealed class TitleStorage : EntityStorage<string, TitleStorageModel>, ITitleStorage
	{
		/// <summary>
		/// Initializes new instance of <see cref="TitleStorage"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		public TitleStorage(IReliableStateManager stateManager)
			: base(stateManager, repositoryStorageKey: "BookstoreStorage", storageFileFullName: GetStorageFileFullName())
		{
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			CancellationToken operationCt = CreateOperationCt(out CancellationTokenSource operationCts);

			Task<IEnumerable<BookstoreTitle>> readAllTitlesTask = GetAllTitlesInternal(operationCts);
			readAllTitlesTask.ThrowOnFailure((task) => new OperationFailedException());

			return readAllTitlesTask;
		}

		/// <inheritdoc/>
		protected override string GetEntityKey(TitleStorageModel entity)
		{
			return entity.Name;
		}

		/// <inheritdoc/>
		protected override IStorageLoader<TitleStorageModel> CreateStorageFileLoader()
		{
			return new JSONStorageLoader<TitleStorageModel>(storageFileFullName);
		}

		/// <summary>
		/// Asynchronously gets all titles known by the bookstore.
		/// </summary>
		/// <returns>All titles known by the bookstore.</returns>
		private async Task<IEnumerable<BookstoreTitle>> GetAllTitlesInternal(CancellationTokenSource operationCts)
		{
			List<BookstoreTitle> allTitles = new List<BookstoreTitle>();

			using (var tx = stateManager.CreateTransaction())
			{
				var titlesFromStorageAsync = await entityRepository.CreateEnumerableAsync(tx);
				var iteratorAsync = titlesFromStorageAsync.GetAsyncEnumerator();

				while (await iteratorAsync.MoveNextAsync(operationCts.Token))
				{
					TitleStorageModel bookstoreTitle = iteratorAsync.Current.Value;

					BookstoreTitle bookTitle = new BookstoreTitle(bookstoreTitle.Name);
					allTitles.Add(bookTitle);
				}

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return allTitles;
		}

		/// <summary>
		/// Gets storage file full name.
		/// </summary>
		/// <returns>Storage file full name.</returns>
		private static string GetStorageFileFullName()
		{
			return Path.Combine(Environment.CurrentDirectory, $"PackageRoot\\AdditionalFiles\\BookstoreDBFake.json");
		}
	}
}
