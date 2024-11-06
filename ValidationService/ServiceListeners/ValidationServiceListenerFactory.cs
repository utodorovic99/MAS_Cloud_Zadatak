using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Fabric.Description;

namespace ValidationService.ServiceListeners
{
	/// <summary>
	/// Factory for creating validation service listeners.
	/// </summary>
	internal static class ValidationServiceListenerFactory
	{
		private static readonly Dictionary<EndpointProtocol, Func<EndpointResourceDescription, ICommunicationListener>> listenerInstantiationsPerProtocol;

		/// <summary>
		/// Static initialization of <see cref="ValidationServiceListenerFactory"/>.
		/// </summary>
		static ValidationServiceListenerFactory()
		{
			listenerInstantiationsPerProtocol = new Dictionary<EndpointProtocol, Func<EndpointResourceDescription, ICommunicationListener>>(1);
			listenerInstantiationsPerProtocol[EndpointProtocol.Http] = (endpoint) => new ValidationServiceHttpListener(endpoint);
		}

		/// <summary>
		/// Creates set of service listeners for each one from <paramref name="endpointsByProtocol"/>.
		/// </summary>
		/// <param name="endpointsByProtocol">Endpoints aggregated by protocol type.</param>
		/// <returns>Set of service listeners for each one from <paramref name="endpointsByProtocol"/>.</returns>
		public static IEnumerable<ServiceInstanceListener> CreateListeners(Dictionary<EndpointProtocol, List<EndpointResourceDescription>> endpointsByProtocol)
		{
			foreach (var endpointsProtocolPair in endpointsByProtocol)
			{
				if (!listenerInstantiationsPerProtocol.TryGetValue(endpointsProtocolPair.Key, out var instantiationFunc))
				{
					continue;
				}

				foreach (var endpoint in endpointsProtocolPair.Value)
				{
					ICommunicationListener customListener = instantiationFunc.Invoke(endpoint);

					yield return new ServiceInstanceListener(context =>
						customListener,
						endpoint.Name); //Currently we are using single request handler per instance.
				}
			}
		}
	}
}
