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

namespace CommunicationsSDK.Proxies
{
	/// <summary>
	/// Manager for service proxies used for internal communication.
	/// </summary>
	public sealed class ServiceProxyManager : IServiceProxyProvider
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
		/// Creates proxies for service of type <typeparamref name="T"/> running on <paramref name="serviceUri"/>.
		/// </summary>
		/// <typeparam name="T">Type of service contract to create.</typeparam>
		/// <param name="serviceUri">Service address.</param>
		public void CreateStatefullProxiesFor<T>(Uri serviceUri)
			where T : IService
		{
			Type contractType = typeof(T);

			ThrowIfInvalidType(contractType);
			ThrowIfAldreadyRegistered(contractType);

			int[] partitionIds = GetAllPartitionIds(serviceUri);

			//Our data model is small so we will keep it on single partition.
			serviceProxiesByContractType[contractType] = ServiceProxy.Create<T>(serviceUri, new ServicePartitionKey(partitionIds[0]), TargetReplicaSelector.PrimaryReplica);
		}

		/// <summary>
		/// Creates proxies for service of type <typeparamref name="T"/> running on <paramref name="serviceUri"/>.
		/// </summary>
		/// <typeparam name="T">Type of service contract to create.</typeparam>
		/// <param name="serviceUri">Service address.</param>
		/// <param name="targetReplica">Ser.</param>
		public void CreateStatelessProxiesFor<T>(Uri serviceUri)
			where T : IService
		{
			Type contractType = typeof(T);

			ThrowIfInvalidType(contractType);
			ThrowIfAldreadyRegistered(contractType);

			//Our data model is small so we will keep it on single partition.
			serviceProxiesByContractType[contractType] = ServiceProxy.Create<T>(serviceUri);
		}

		/// <inheritdoc/>
		public T GetProxyFor<T>() where T : IService
		{
			Type contractType = typeof(T);
			ThrowIfNotRegistered(contractType);
			return (T)serviceProxiesByContractType[contractType];
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
