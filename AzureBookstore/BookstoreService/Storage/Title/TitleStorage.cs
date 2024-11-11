using BookstoreService.Storage.Exceptions;
using BookstoreServiceContracts.Model;
using CommunicationsSDK.PlatformExtensions;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreService.Storage.Title
{
	/// <summary>
	/// Class representing storage of titles.
	/// </summary>
	internal sealed class TitleStorage : ITitleStorage
	{
		private const int MaxWaitTimeoutMs = 10000;
		private const string BookRepositoryStorageKey = "BookstoreStorage";
		private readonly IReliableStateManager stateManager;

		private IReliableDictionary<string, TitleStorageModel> bookRepository;
		private CancellationTokenSource storageCts;
		private CancellationToken storageCt;

		/// <summary>
		/// Initializes new instance of <see cref="TitleStorage"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		public TitleStorage(IReliableStateManager stateManager)
		{
			this.stateManager = stateManager;
			storageCts = new CancellationTokenSource();
			storageCt = storageCts.Token;
		}

		/// <inheritdoc/>
		public void InitializeStorage()
		{
			InitializeStorageAsync();
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			CancellationToken operationCt = CreateOperationCt(out CancellationTokenSource operationCts);

			Task<IEnumerable<BookstoreTitle>> readAllTitlesTask = GetAllTitlesInternal(operationCts);
			readAllTitlesTask.ThrowOnFailure((task) => new OperationFailedException());

			return readAllTitlesTask;
		}

		/// <summary>
		/// Asynchronously gets all titles known by the bookstore.
		/// </summary>
		/// <returns>All titles known by the bookstore.</returns>
		public async Task<IEnumerable<BookstoreTitle>> GetAllTitlesInternal(CancellationTokenSource operationCts)
		{
			List<BookstoreTitle> allTitles = new List<BookstoreTitle>();

			using (var tx = stateManager.CreateTransaction())
			{
				var titlesFromStorageAsync = await bookRepository.CreateEnumerableAsync(tx);
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
		/// Asynchronously initializes storage.
		/// </summary>
		private async void InitializeStorageAsync()
		{
			bookRepository = await stateManager.GetOrAddAsync<IReliableDictionary<string, TitleStorageModel>>(BookRepositoryStorageKey);

			string storageFileName = "BookstoreDBFake.json";

			using (var loader = new JSONStorageLoader<TitleStorageModel>(storageFileName))
			{
				loader.Load();

				using (var tx = stateManager.CreateTransaction())
				{
					var iterator = loader.LoadedData.GetEnumerator();
					while (iterator.MoveNext())
					{
						TitleStorageModel title = iterator.Current;

						await bookRepository.AddOrUpdateAsync(tx, title.Name, title, (k, v) => title);
					}

					await tx.CommitAsync();
				}
			}
		}

		/// <summary>
		/// Creates new cancellation token scoped for single API operation.
		/// </summary>
		/// <returns>New cancellation token scoped for single API operation.</returns>
		/// <remarks>
		/// New cancellation token is derived from overall storage cancellation token.
		/// Token is auto-canceled after predefined period of time.
		/// </remarks>
		private CancellationToken CreateOperationCt()
		{
			return CreateOperationCt(out _);
		}

		/// <summary>
		/// Creates new cancellation token scoped for single API operation.
		/// </summary>
		/// <param name="operationCts">Cancellation token source as owner of returned cancellation token.</param>
		/// <returns>New cancellation token scoped for single API operation.</returns>
		/// <remarks>
		/// New cancellation token is derived from overall storage cancellation token.
		/// Token is auto-canceled after predefined period of time.
		/// </remarks>
		private CancellationToken CreateOperationCt(out CancellationTokenSource operationCts)
		{
			operationCts = CancellationTokenSource.CreateLinkedTokenSource(storageCt);
			operationCts.CancelAfter(MaxWaitTimeoutMs);

			return operationCts.Token;
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			storageCts.Cancel();
			storageCts.Dispose();
		}
	}
}
