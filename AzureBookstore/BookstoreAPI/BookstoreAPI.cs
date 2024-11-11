using BookstoreAPI.FabricClients;
using BookstoreAPI.Listeners;
using BookstoreServiceContracts.Model;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreAPI
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class BookstoreAPI : StatelessService
	{
		/// <summary>
		/// Initializes new instance of <see cref="BookstoreAPI"/>
		/// </summary>
		/// <param name="context">Service context.</param>
		public BookstoreAPI(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
		/// </summary>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			IEnumerable<EndpointResourceDescription> endpoints = this.Context.CodePackageActivationContext.GetEndpoints();
			return ListenerFactory.CreateFor(endpoints);
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
		/// Gets all titles.
		/// </summary>
		/// <param name="purchase"></param>
		/// <returns>Task with collection of titles known by the bookstore.</returns>
		public async Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			BookstoreFabricClient fabricClient = new BookstoreFabricClient();
			return await fabricClient.GetAllTitles();
		}

		///// <summary>
		///// Request handler for purchase of book titles.
		///// </summary>
		///// <param name="purchase">Title purchase.</param>
		///// <returns>Purchase result.</returns>
		//public async Task<PurchaseResponse> PurchaseTitle(PurchaseRequest purchase)
		//{
		//	PurchaseResponse purchaseResponse = new PurchaseResponse();

		//	PurchaseValidationResponse validationResponse = await validationServiceProxy.ValidatePurchaseRequest(purchase);
		//	if (!validationResponse.ValidityStatus.Equals(PurchaseValidityStatus.Valid))
		//	{
		//		purchaseResponse.Status = StatusHelper.ToResponseStatus(validationResponse.ValidityStatus);
		//	}

		//	purchaseResponse.Status = PurchaseResponseStatus.Success;
		//	return purchaseResponse;
		//}
	}
}
