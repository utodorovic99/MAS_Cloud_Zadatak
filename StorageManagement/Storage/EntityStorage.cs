using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Threading;

namespace StorageManagement
{
	/// <summary>
	/// Class representing storage of entities of type <typeparamref name="TValue"/>
	/// identified by <typeparamref name="TKey"/>.
	/// </summary>
	/// <typeparam name="TKey">Type of entity key.</typeparam>
	/// <typeparam name="TValue">Type of entities stored in storage.</typeparam>
	public abstract class EntityStorage<TKey, TValue> : IStorage
		where TKey : IComparable<TKey>, IEquatable<TKey>
		where TValue : class
	{
		private const int MaxWaitTimeoutMs = 30000;

		protected readonly IReliableStateManager stateManager;
		protected readonly string repositoryStorageKey;
		protected readonly string storageFileFullName;
		protected IReliableDictionary<TKey, TValue> entityRepository;

		private CancellationTokenSource storageCts;
		private CancellationToken storageCt;

		/// <summary>
		/// Initializes new instance of <see cref="EntityStorage{TKey, TValue}"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		/// <param name="repositoryStorageKey">Unique storage identifier used by state manager.</param>
		public EntityStorage(IReliableStateManager stateManager, string repositoryStorageKey, string storageFileFullName)
		{
			this.stateManager = stateManager;
			this.repositoryStorageKey = repositoryStorageKey;
			this.storageFileFullName = storageFileFullName;
			storageCts = new CancellationTokenSource();
			storageCt = storageCts.Token;
		}

		/// <inheritdoc/>
		public void InitializeStorage()
		{
			InitializeStorageAsync();
		}

		/// <inheritdoc/>
		public virtual void Dispose()
		{
			storageCts.Cancel();
			storageCts.Dispose();
		}

		/// <summary>
		/// Creates storage file loader.
		/// </summary>
		/// <returns>Storage file loader.</returns>
		protected abstract IStorageLoader<TValue> CreateStorageFileLoader();

		/// <summary>
		/// Gets key for <paramref name="entity"/>.
		/// </summary>
		/// <param name="entity">Entity whose key is requested.</param>
		/// <returns>Key for <paramref name="entity"/>.</returns>
		protected abstract TKey GetEntityKey(TValue entity);

		/// <summary>
		/// Creates new cancellation token scoped for single storage async operation.
		/// </summary>
		/// <returns>New cancellation token scoped for single storage async operation.</returns>
		/// <remarks>
		/// New cancellation token is derived from overall storage cancellation token.
		/// Token is auto-canceled after predefined period of time.
		/// </remarks>
		protected CancellationToken CreateOperationCt()
		{
			return CreateOperationCt(out _);
		}

		/// <summary>
		/// Creates new cancellation token scoped for single storage async operation.
		/// </summary>
		/// <param name="operationCts">Cancellation token source as owner of returned cancellation token.</param>
		/// <returns>New cancellation token scoped for single storage async operation.</returns>
		/// <remarks>
		/// New cancellation token is derived from overall storage cancellation token.
		/// Token is auto-canceled after predefined period of time.
		/// </remarks>
		protected CancellationToken CreateOperationCt(out CancellationTokenSource operationCts)
		{
			operationCts = CancellationTokenSource.CreateLinkedTokenSource(storageCt);
			operationCts.CancelAfter(MaxWaitTimeoutMs);

			return operationCts.Token;
		}

		/// <summary>
		/// Asynchronously initializes storage.
		/// </summary>
		protected virtual async void InitializeStorageAsync()
		{
			entityRepository = await stateManager.GetOrAddAsync<IReliableDictionary<TKey, TValue>>(repositoryStorageKey);

			using (IStorageLoader<TValue> loader = CreateStorageFileLoader())
			{
				loader.Load();

				using (var tx = stateManager.CreateTransaction())
				{
					var iterator = loader.LoadedData.GetEnumerator();
					while (iterator.MoveNext())
					{
						TValue loadedEntity = iterator.Current;

						await entityRepository.AddOrUpdateAsync(tx, GetEntityKey(loadedEntity), loadedEntity, (k, v) => loadedEntity);
					}

					await tx.CommitAsync();
				}
			}
		}
	}
}
