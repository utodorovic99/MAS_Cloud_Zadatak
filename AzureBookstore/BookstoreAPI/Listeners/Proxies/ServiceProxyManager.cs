using BookstoreServiceContract.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreAPI.Listeners
{
	/// <summary>
	/// Manager for service proxies used for internal communication.
	/// </summary>
	internal sealed class ServiceProxyManager : IServiceProxyProvider
	{
		private const int MaxWaitTimeoutMs = 5000;

		private readonly Dictionary<Type, IService> serviceProxiesByContractType;

		/// <summary>
		/// Initializes new instance of <see cref="ServiceProxyManager"/>.
		/// </summary>
		public ServiceProxyManager()
		{
			serviceProxiesByContractType = new Dictionary<Type, IService>();
		}

		/// <summary>
		/// Creates proxies for <paramref name="contractType"/> running on <paramref name="serviceUri"/>.
		/// </summary>
		/// <param name="contractType">Type of service contract.</param>
		/// <param name="serviceUri">Service address.</param>
		public void CreateProxiesFor(Type contractType, Uri serviceUri)
		{
			ThrowIfInvalidType(contractType);
			ThrowIfAldreadyRegistered(contractType);

			int[] partitionIds = GetAllPartitionIds(serviceUri);

			//Our data model is small so we will keep it on single partition.
			serviceProxiesByContractType[contractType] = ServiceProxy.Create<IBookstoreServiceContract>(Program.Configuration.BookstoreServiceUri, new ServicePartitionKey(partitionIds[0]), TargetReplicaSelector.PrimaryReplica); ;
		}

		/// <inheritdoc/>
		public IService GetProxyFor(Type contractType)
		{
			ThrowIfNotRegistered(contractType);
			return serviceProxiesByContractType[contractType];
		}

		/// <summary>
		/// Gets all partition ids for service addressed by on <paramref name="serviceUri"/>.
		/// </summary>
		/// <param name="serviceUri">Service address.</param>
		/// <returns>Collection of partition ids for service addressed by on <paramref name="serviceUri"/>.</returns>
		/// <exception cref="ApplicationException"> in case remote query for partition ids failed.</exception>
		private int[] GetAllPartitionIds(Uri serviceUri)
		{
			FabricClient fabricClient = new FabricClient();

			Task<ServicePartitionList> getAllPartitionsTask = fabricClient.QueryManager.GetPartitionListAsync(serviceUri);
			if (!getAllPartitionsTask.Wait(MaxWaitTimeoutMs))
			{
				throw new ApplicationException($"Retrieving partitions of: '{serviceUri}' failed."); //We are letting service to fail here.
			}

			return Enumerable.Range(0, getAllPartitionsTask.Result.Count).Select(x => x % getAllPartitionsTask.Result.Count).ToArray();
		}


		/// <summary>
		/// Throws <see cref="ArgumentException"/> if <paramref name="contractType"/> does not derive from <see cref="IService"/>.
		/// </summary>
		/// <param name="contractType">Contract type to check.</param>
		/// <exception cref="ArgumentException">if <paramref name="contractType"/> does not derive from <see cref="IService"/>.</exception>
		private void ThrowIfInvalidType(Type contractType)
		{
			if (!typeof(IService).IsAssignableFrom(contractType))
			{
				throw new ArgumentException(nameof(contractType), $"Type must derive from: {typeof(IService).FullName}");
			}
		}

		/// <summary>
		/// Throws <see cref="ArgumentException"/> if service proxy for <paramref name="contractType"/> is already registered.
		/// </summary>
		/// <param name="contractType">Contract type to check.</param>
		/// <exception cref="ArgumentException"> if service proxy for <paramref name="contractType"/> is already registered.</exception>
		private void ThrowIfAldreadyRegistered(Type contractType)
		{
			if (serviceProxiesByContractType.ContainsKey(contractType))
			{
				throw new ArgumentException(nameof(contractType), $"Proxy already initialized for: {typeof(IService).FullName}");
			}
		}

		/// <summary>
		/// Throws <see cref="ArgumentException"/> if service proxy for <paramref name="contractType"/> is not registered.
		/// </summary>
		/// <param name="contractType">Contract type to check.</param>
		/// <exception cref="ArgumentException"> if service proxy for <paramref name="contractType"/> is not registered.</exception>
		private void ThrowIfNotRegistered(Type contractType)
		{
			if (!serviceProxiesByContractType.ContainsKey(contractType))
			{
				throw new ArgumentException(nameof(contractType), $"Proxy not registered for: {typeof(IService).FullName}");
			}
		}
	}
}
