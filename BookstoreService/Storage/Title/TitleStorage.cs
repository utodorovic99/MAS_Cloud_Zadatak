using BookstoreServiceContract.Enums;
using BookstoreServiceContract.Model;
using Common.Model;
using CommunicationsSDK.PlatformExtensions;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
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
		private readonly string enlistedPurchasesStorageKey;
		private IReliableDictionary<uint, PurchaseStorageModel> inProgressBookPurchases;

		/// <summary>
		/// Initializes new instance of <see cref="TitleStorage"/>
		/// </summary>
		/// <param name="stateManager">State manager for state integrity mechanism.</param>
		public TitleStorage(IReliableStateManager stateManager)
			: base(stateManager, repositoryStorageKey: "BookstoreStorage", storageFileFullName: GetStorageFileFullName())
		{
			enlistedPurchasesStorageKey = "TitlePurchasesInProgress";
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
		public async Task<BookstoreTitle> GetTitle(string titleName)
		{
			BookstoreTitle resultTitle = null;

			using (var tx = stateManager.CreateTransaction())
			{
				ConditionalValue<TitleStorageModel> titleInStorage = await entityRepository.TryGetValueAsync(tx, titleName);
				if (titleInStorage.HasValue)
				{
					TitleStorageModel bookstoreTitle = titleInStorage.Value;
					resultTitle = new BookstoreTitle()
					{
						Name = bookstoreTitle.Name,
						Price = bookstoreTitle.Price,
					};
				}

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return resultTitle;
		}

		/// <inheritdoc/>
		public async Task<bool> Exists(string titleName)
		{
			bool titleExists = false;

			using (var tx = stateManager.CreateTransaction())
			{
				titleExists = await entityRepository.ContainsKeyAsync(tx, titleName);

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return titleExists;
		}

		/// <inheritdoc/>
		public async Task<BookstoreEnlistPurchaseResult> EnlistBookForPurchase(string bookId)
		{
			BookstoreEnlistPurchaseResult enlistmentResult;

			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					enlistmentResult = await EnlistBookPurchaseInternal(tx, bookId);
					await tx.CommitAsync();
					return enlistmentResult;
				}
				catch
				{
					//Do not commit and let the azure fabric infrastructure to rollback transaction automatically.
				}
			}

			return new BookstoreEnlistPurchaseResult()
			{
				Status = BookstoreEnlistPurchaseStatus.Fail,
				PurchaseId = 0,
			};
		}

		/// <inheritdoc/>
		public async Task<bool> ConfirmEnlistedPurchase(uint purchaseId)
		{
			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					await inProgressBookPurchases.TryRemoveAsync(tx, purchaseId);
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
		public async Task<bool> RevokeEnlistedPurchase(uint purchaseId)
		{
			using (var tx = stateManager.CreateTransaction())
			{
				try
				{
					ConditionalValue<PurchaseStorageModel> revokedPurchaseFromStorage = await inProgressBookPurchases.TryRemoveAsync(tx, purchaseId);
					if (!revokedPurchaseFromStorage.HasValue)
					{
						return false;
					}

					PurchaseStorageModel revokedPurchase = revokedPurchaseFromStorage.Value;

					ConditionalValue<TitleStorageModel> titleInStorage = await entityRepository.TryGetValueAsync(tx, revokedPurchase.Title);
					if (!titleInStorage.HasValue)
					{
						return false;
					}

					TitleStorageModel title = titleInStorage.Value;
					title.Copies += 1;

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
		protected override string GetEntityKey(TitleStorageModel entity)
		{
			return entity.Name;
		}

		/// <inheritdoc/>
		protected override IStorageLoader<TitleStorageModel> CreateStorageFileLoader()
		{
			return new JSONStorageLoader<TitleStorageModel>(storageFileFullName);
		}

		/// <inheritdoc/>
		protected async override void InitializeStorageAsync()
		{
			base.InitializeStorageAsync();
			inProgressBookPurchases = await stateManager.GetOrAddAsync<IReliableDictionary<uint, PurchaseStorageModel>>(enlistedPurchasesStorageKey);
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

					BookstoreTitle bookTitle = new BookstoreTitle()
					{
						Name = bookstoreTitle.Name,
						Price = bookstoreTitle.Price,
					};

					allTitles.Add(bookTitle);
				}

				// Not necessary but kept to keep up with transaction semantics.
				await tx.CommitAsync();
			}

			return allTitles;
		}

		/// <summary>
		/// Executes internal logic for enlisting purchase of <paramref name="bookId"/>.
		/// </summary>
		/// <param name="transaction">Transaction under which operation is being performed.</param>
		/// <param name="bookId">Book identifier.</param>
		/// <returns>Result indicating result of book purchase enlistment.</returns>
		private async Task<BookstoreEnlistPurchaseResult> EnlistBookPurchaseInternal(ITransaction transaction, string bookId)
		{
			ConditionalValue<TitleStorageModel> titleInStorage = await entityRepository.TryGetValueAsync(transaction, bookId);
			if (!titleInStorage.HasValue)
			{
				return new BookstoreEnlistPurchaseResult()
				{
					Status = BookstoreEnlistPurchaseStatus.Fail,
					PurchaseId = 0,
				};
			}

			TitleStorageModel title = titleInStorage.Value;
			if (title.Copies <= 0)
			{
				return new BookstoreEnlistPurchaseResult()
				{
					Status = BookstoreEnlistPurchaseStatus.OutOfCopies,
					PurchaseId = 0,
				};
			}

			title.Copies -= 1;
			uint purchaseId = PurchaseIdGenerator.Generate();
			PurchaseStorageModel purchase = new PurchaseStorageModel()
			{
				PurchaseId = purchaseId,
				Title = bookId,
			};

			await inProgressBookPurchases.AddAsync(transaction, purchase.PurchaseId, purchase);

			return new BookstoreEnlistPurchaseResult()
			{
				Status = BookstoreEnlistPurchaseStatus.Success,
				PurchaseId = purchaseId,
			};
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
