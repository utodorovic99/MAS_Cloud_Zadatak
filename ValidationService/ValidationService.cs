using BookstoreServiceContract.Contracts;
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
using UsersServiceContract.Contract;
using ValidationDataContract;
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
		/// <param name="context"></param>
		public ValidationService(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <inheritdoc/>
		public async Task<PurchaseValidityStatus> ValidatePurchaseRequest(PurchaseRequest purchaseRequest)
		{
			try
			{
				IBookstoreServiceContract bookstoreService = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
				bool titleExists = await bookstoreService.CheckTitleExists(purchaseRequest.Title);
				if (!titleExists)
				{
					return PurchaseValidityStatus.UnknownBookTitle;
				}

				IUsersServiceContract usersService = proxyProvider.GetProxyFor<IUsersServiceContract>();
				bool userExists = await usersService.CheckUsernameExists(purchaseRequest.User);
				if (!userExists)
				{
					return PurchaseValidityStatus.InvalidUser;
				}

				return PurchaseValidityStatus.Valid;
			}
			catch (Exception e)
			{
				return PurchaseValidityStatus.Invalid;
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
		}
	}
}
