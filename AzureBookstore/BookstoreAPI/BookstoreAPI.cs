using BookstoreAPI.Listeners;
using BookstoreServiceContract.Contracts;
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

			ServiceProxyManager proxyManager = new ServiceProxyManager();
			CreateServiceProxies(proxyManager);

			return ListenerFactory.CreateFor(endpoints, proxyManager);
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
			proxyManager.CreateProxiesFor(typeof(IBookstoreServiceContract), Program.Configuration.BookstoreServiceUri);
		}
	}
}
