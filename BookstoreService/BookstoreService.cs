using BookstoreService.Storage.Title;
using BookstoreServiceContract.Contracts;
using BookstoreServiceContract.Model;
using Common.Model;
using CommunicationsSDK.Listeners;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreService
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class BookstoreService : StatefulService, IBookstoreServiceContract
	{
		private ITitleStorage titleStorage;

		/// <summary>
		/// Initializes new instance of <see cref="BookstoreService"/>
		/// </summary>
		/// <param name="context">Service context.</param>
		public BookstoreService(StatefulServiceContext context)
			: base(context)
		{
		}

		/// <inheritdoc/>
		public Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			return titleStorage.GetAllTitles();
		}

		/// <inheritdoc/>
		public Task<BookstoreTitle> GetTitle(string titleName)
		{
			return titleStorage.GetTitle(titleName);
		}

		/// <inheritdoc/>
		public Task<bool> CheckTitleExists(string titleName)
		{
			return titleStorage.Exists(titleName);
		}

		/// <inheritdoc/>
		public Task<BookstoreEnlistPurchaseResult> EnlistBookForPurchase(string bookId)
		{
			return titleStorage.EnlistBookForPurchase(bookId);
		}

		/// <inheritdoc/>
		public Task<bool> ConfirmEnlistedPurchase(uint purchaseId)
		{
			return titleStorage.ConfirmEnlistedPurchase(purchaseId);
		}

		/// <inheritdoc/>
		public Task<bool> RevokeEnlistedPurchase(uint purchaseId)
		{
			return titleStorage.RevokeEnlistedPurchase(purchaseId);
		}

		/// <summary>
		/// This is the main entry point for your service replica.
		/// This method executes when this replica of your service becomes primary and has write status.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			titleStorage = new TitleStorage(StateManager);
			titleStorage.InitializeStorage();

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}
		}

		/// <inheritdoc/>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			IEnumerable<EndpointResourceDescription> endpoints = this.Context.CodePackageActivationContext.GetEndpoints();
			return RpcListenerFactory.CreateForStatefull(this, endpoints);
		}
	}
}
