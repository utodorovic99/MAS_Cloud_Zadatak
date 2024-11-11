using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V1.FabricTransport.Runtime;
using System.Collections.Generic;
using System.Fabric.Description;

namespace BookstoreService.Listeners
{
	/// <summary>
	/// Class for creating <see cref="ServiceInstanceListener"/> type of objects.
	/// </summary>
	internal static class ListenerFactory
	{
		/// <summary>
		/// Creates <see cref="ServiceReplicaListener"/> for each one of <paramref name="endpoints"/>.
		/// </summary>
		/// <param name="service">Name of service whose endpoints are exposed.</param>
		/// <param name="endpoints">Set of endpoints for which listeners will be created.</param>
		/// <returns>Collection of <see cref="ServiceInstanceListener"/> for provided <paramref name="endpoints"/>.</returns>
		public static IEnumerable<ServiceReplicaListener> CreateFor(IService service, IEnumerable<EndpointResourceDescription> endpoints)
		{
			foreach (EndpointResourceDescription endpoint in endpoints)
			{
				if (IsRpcEndpoint(endpoint))
				{
					yield return new ServiceReplicaListener((c) =>
					{
						return new FabricTransportServiceRemotingListener(c, service);
					}, name: endpoint.Name);
				}
			}
		}

		/// <summary>
		/// Checks whether remote point is used for remote procedure calls.
		/// </summary>
		/// <param name="endpoint">RPC endpoint candidate</param>
		/// <returns><c>True</c> if <paramref name="endpoint"/> is an RPC endpoint.</returns>
		private static bool IsRpcEndpoint(EndpointResourceDescription endpoint)
		{
			return endpoint.Name.StartsWith("Rpc_", System.StringComparison.OrdinalIgnoreCase);
		}
	}
}
