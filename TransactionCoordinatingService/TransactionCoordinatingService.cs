using BookstoreServiceContract.Contracts;
using BookstoreServiceContract.Enums;
using BookstoreServiceContract.Model;
using Common.Enums;
using Common.Model;
using CommunicationsSDK.Listeners;
using CommunicationsSDK.Proxies;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;
using TransactionCoordinatingService.Helpers;
using TransactionCoordinatingServiceContract.Contract;
using UsersServiceContract.Contract;
using UsersServiceContract.Enums;

namespace TransactionCoordinatingService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class TransactionCoordinatingService : StatelessService, ITransactionCoordinatingServiceContract
	{
		private IServiceProxyProvider proxyProvider;

		/// <summary>
		/// Initializes new instance of <see cref="TransactionCoordinatingService"/>.
		/// </summary>
		/// <param name="context">Service context.</param>
		public TransactionCoordinatingService(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <inheritdoc/>
		public async Task<PurchaseResponse> ExecuteCoordinatedPurchase(PurchaseRequest purchaseRequest)
		{
			bool operationResult = false;
			PurchaseResponse invalidResponse = new PurchaseResponse()
			{
				Status = PurchaseResponseStatus.Fail,
			};

			//TODO: Wrap with some try catches.

			//Prepare money
			EnlistMoneyTransferResult prepareMoneyResult = await PrepareMoney(purchaseRequest);
			if (!prepareMoneyResult.Status.Equals(EnlistMoneyTransferStatus.Success))
			{
				invalidResponse.Status = StatusMapper.GetPurchaseResponseStatus(prepareMoneyResult.Status);
				return invalidResponse;
			}

			//Prepare book
			BookstoreEnlistPurchaseResult prepareBookResult = await PrepareTitleCopy(purchaseRequest);
			if (!prepareBookResult.Status.Equals(BookstoreEnlistPurchaseStatus.Success))
			{
				//Revoke money if book failed
				operationResult = await RevokeMoney(prepareMoneyResult.TransactionId);
				if (!operationResult)
				{
					//Log properly and handle by secondary mechanism.
				}

				invalidResponse.Status = StatusMapper.GetPurchaseResponseStatus(prepareBookResult.Status);
				return invalidResponse;
			}

			//Commit money
			operationResult = await CommitMoney(prepareMoneyResult.TransactionId);
			if (!operationResult)
			{
				//Revoke money if its commit failed
				operationResult = await RevokeMoney(prepareMoneyResult.TransactionId);
				if (!operationResult)
				{
					//Log properly and handle by secondary mechanism.
				}

				//Revoke book if money commit failed
				operationResult = await RevokeBook(prepareBookResult.PurchaseId);
				if (!operationResult)
				{
					//Log properly and handle by secondary mechanism.
				}

				return invalidResponse;
			}

			//Commit book
			operationResult = await CommitTitleCopy(prepareBookResult.PurchaseId);
			if (!operationResult)
			{
				//Revoke money if its commit failed
				operationResult = await RevokeMoney(prepareMoneyResult.TransactionId);
				if (!operationResult)
				{
					//Log properly and handle by secondary mechanism.
				}

				return invalidResponse;
			}

			return new PurchaseResponse()
			{
				Status = PurchaseResponseStatus.Success
			};
		}

		/// <summary>
		/// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
		/// </summary>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			IEnumerable<EndpointResourceDescription> endpoints = this.Context.CodePackageActivationContext.GetEndpoints();

			ServiceProxyManager proxyManager = new ServiceProxyManager();
			CreateServiceProxies(proxyManager);
			proxyProvider = proxyManager;

			return RpcListenerFactory.CreateForStateless(this, endpoints);
		}

		/// <summary>
		/// This is the main entry point for your service instance.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}
		}

		/// <summary>
		/// Executes 'Prepare' transaction operation for money.
		/// </summary>
		/// <param name="purchaseRequest">Purchase request.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<EnlistMoneyTransferResult> PrepareMoney(PurchaseRequest purchaseRequest)
		{
			IUsersServiceContract usersServiceProxy = proxyProvider.GetProxyFor<IUsersServiceContract>();
			IBookstoreServiceContract bookstoreServiceProxy = proxyProvider.GetProxyFor<IBookstoreServiceContract>();

			BookstoreTitle bookTitle = await bookstoreServiceProxy.GetTitle(purchaseRequest.Title);
			if (bookTitle is null)
			{
				return new EnlistMoneyTransferResult()
				{
					Status = EnlistMoneyTransferStatus.Fail,
				};
			}

			return await usersServiceProxy.EnlistMoneyTransfer(purchaseRequest.User, bookTitle.Price);
		}

		/// <summary>
		/// Executes 'Prepare' transaction operation for book copy.
		/// </summary>
		/// <param name="purchaseRequest">Purchase request.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<BookstoreEnlistPurchaseResult> PrepareTitleCopy(PurchaseRequest purchaseRequest)
		{
			IBookstoreServiceContract bookstoreServiceProxy = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
			return await bookstoreServiceProxy.EnlistBookForPurchase(purchaseRequest.Title);
		}

		/// <summary>
		/// Executes 'Commit' transaction operation for money.
		/// </summary>
		/// <param name="transactionId">Transaction unique identifier.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<bool> CommitMoney(uint transactionId)
		{
			IUsersServiceContract usersServiceProxy = proxyProvider.GetProxyFor<IUsersServiceContract>();
			return await usersServiceProxy.ConfirmEnlistedMoneyTransfer(transactionId);
		}

		/// <summary>
		/// Executes 'Commit' transaction operation for title copy.
		/// </summary>
		/// <param name="purchaseId">Unique id of purchase.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<bool> CommitTitleCopy(uint purchaseId)
		{
			IBookstoreServiceContract bookstoreServiceProxy = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
			return await bookstoreServiceProxy.ConfirmEnlistedPurchase(purchaseId);
		}

		/// <summary>
		/// Executes 'Rollback' transaction operation for money.
		/// </summary>
		/// <param name="transactionId">Transaction unique identifier.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<bool> RevokeMoney(uint transactionId)
		{
			IUsersServiceContract usersServiceProxy = proxyProvider.GetProxyFor<IUsersServiceContract>();
			return await usersServiceProxy.RevokeEnlistedMoneyTransfer(transactionId);
		}

		/// <summary>
		/// Executes 'Rollback' transaction operation for title copy.
		/// </summary>
		/// <param name="purchaseId">Unique id of purchase.</param>
		/// <returns>Indicator whether operation succeeded.</returns>
		private async Task<bool> RevokeBook(uint purchaseId)
		{
			IBookstoreServiceContract bookstoreServiceProxy = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
			return await bookstoreServiceProxy.RevokeEnlistedPurchase(purchaseId);
		}

		/// <summary>
		/// Creates service proxies required by the service.
		/// </summary>
		/// <param name="proxyManager">Service proxy manager.</param>
		private void CreateServiceProxies(ServiceProxyManager proxyManager)
		{
			proxyManager.CreateStatefullProxiesFor<IBookstoreServiceContract>(Program.Configuration.BookstoreServiceUri);
			proxyManager.CreateStatefullProxiesFor<IUsersServiceContract>(Program.Configuration.UsersServiceUri);
		}
	}
}
