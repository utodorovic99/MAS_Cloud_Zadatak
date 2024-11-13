using CommunicationsSDK.Listeners;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;
using UsersService.Storage.Users;
using UsersServiceContract.Contract;

namespace UsersService
{
	/// <summary>
	/// An instance of this class is created for each service replica by the Service Fabric runtime.
	/// </summary>
	internal sealed class UserService : StatefulService, IUsersServiceContract
	{
		private IUserStorage userStorage;

		/// <summary>
		/// Initializes new instance of <see cref="UserService"/>.
		/// </summary>
		/// <param name="context">Service context.</param>
		public UserService(StatefulServiceContext context)
			: base(context)
		{
		}

		/// <inheritdoc/>
		public async Task<bool> CheckUsernameExists(string username)
		{
			return await userStorage.CheckUsernameExists(username);
		}

		/// <inheritdoc/>
		protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
		{
			IEnumerable<EndpointResourceDescription> endpoints = this.Context.CodePackageActivationContext.GetEndpoints();
			return RpcListenerFactory.CreateFor(this, endpoints);
		}

		/// <summary>
		/// This is the main entry point for your service replica.
		/// This method executes when this replica of your service becomes primary and has write status.
		/// </summary>
		/// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
		protected override async Task RunAsync(CancellationToken cancellationToken)
		{
			userStorage = new UserStorage(StateManager);
			userStorage.InitializeStorage();

			while (true)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
			}
		}
	}
}
