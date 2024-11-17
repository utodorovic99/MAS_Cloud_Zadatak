using BookstoreServiceContract.Contracts;
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
using TransactionCoordinatingServiceContract.Contract;
using UsersServiceContract.Contract;
using ValidationServiceContract.Contract;

namespace ValidationService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class ValidationService : StatelessService, IValidationServiceContract
	{
		private IServiceProxyProvider proxyProvider;

		/// <summary>
		/// Initializes new instance of <see cref="ValidationService"/>.
		/// </summary>
		/// <param name="context">Service context.</param>
		public ValidationService(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <inheritdoc/>
		public async Task<PurchaseResponse> TryExecutePurchase(PurchaseRequest purchaseRequest)
		{
			try
			{
				PurchaseResponse purchaseResponse = new PurchaseResponse();

				bool isRequestValid = await Validate(purchaseRequest, purchaseResponse);
				if (!isRequestValid)
				{
					return purchaseResponse;
				}

				ITransactionCoordinatingServiceContract transactionCoordinatingServiceProxy = proxyProvider.GetProxyFor<ITransactionCoordinatingServiceContract>();

				return await transactionCoordinatingServiceProxy.ExecuteCoordinatedPurchase(purchaseRequest);
			}
			catch
			{
				return new PurchaseResponse()
				{
					Status = PurchaseResponseStatus.Fail,
				};
			}
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
		/// Creates service proxies required by the service.
		/// </summary>
		/// <param name="proxyManager">Service proxy manager.</param>
		private void CreateServiceProxies(ServiceProxyManager proxyManager)
		{
			proxyManager.CreateStatefullProxiesFor<IBookstoreServiceContract>(Program.Configuration.BookstoreServiceUri);
			proxyManager.CreateStatefullProxiesFor<IUsersServiceContract>(Program.Configuration.UsersServiceUri);
			proxyManager.CreateStatelessProxiesFor<ITransactionCoordinatingServiceContract>(Program.Configuration.TransactionCoordinatingServiceUri);
		}

		/// <summary>
		/// Validates <paramref name="purchaseRequest"/>.
		/// </summary>
		/// <param name="purchaseRequest">Request to validate </param>
		/// <param name="purchaseResponse">Response containing error indicator if validation failed.</param>
		/// <returns><c>True</c> if validation succeeded; <c>false</c> otherwise.</returns>
		private async Task<bool> Validate(PurchaseRequest purchaseRequest, PurchaseResponse purchaseResponse)
		{
			IBookstoreServiceContract bookstoreService = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
			bool titleExists = await bookstoreService.CheckTitleExists(purchaseRequest.Title);
			if (!titleExists)
			{
				purchaseResponse.Status = PurchaseResponseStatus.UnknownBookTitle;
				return false;
			}

			IUsersServiceContract usersService = proxyProvider.GetProxyFor<IUsersServiceContract>();
			bool userExists = await usersService.CheckUsernameExists(purchaseRequest.User);
			if (!userExists)
			{
				purchaseResponse.Status = PurchaseResponseStatus.InvalidUser;
				return false;
			}

			return true;
		}
	}
}
