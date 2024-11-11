using BookstoreService.Listeners;
using BookstoreService.Storage.Title;
using BookstoreServiceContracts.Contracts;
using BookstoreServiceContracts.Model;
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
		public async Task<IEnumerable<BookstoreTitle>> GetAllTitles()
		{
			return await titleStorage.GetAllTitles();
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
			return ListenerFactory.CreateFor(this, endpoints);
		}
	}
}
