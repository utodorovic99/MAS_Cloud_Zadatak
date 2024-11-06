using System.Fabric;
using System.Fabric.Description;

namespace ValidationService.Helpers
{
	internal static class ServiceManifestHelper
	{
		/// <summary>
		/// Gets validation endpoints per protocol type.
		/// </summary>
		/// <param name="serviceContext">Service context where endpoints are defined.</param>
		/// <returns>Validation endpoints per protocol type map.</returns>
		public static Dictionary<EndpointProtocol, List<EndpointResourceDescription>> GetValidationServiceEndpointsByProtocol(ServiceContext serviceContext)
		{
			List<EndpointResourceDescription> serviceEndpoints = serviceContext.CodePackageActivationContext.GetEndpoints().ToList();
			HashSet<EndpointProtocol> supportedEndpointProtocols = GetSupportedEndpointProtocols();
			var endpointsByProtocol = CreateEmptyProtocolToEdnpointMap(supportedEndpointProtocols);

			foreach (EndpointResourceDescription endpoint in serviceEndpoints)
			{
				if (!endpoint.EndpointType.Equals(EndpointType.Internal)
					|| !endpoint.Name.StartsWith("ValidationEndpoint")
					|| !supportedEndpointProtocols.Contains(endpoint.Protocol))
				{
					continue;
				}

				endpointsByProtocol[endpoint.Protocol].Add(endpoint);
			}

			return endpointsByProtocol;
		}

		/// <summary>
		/// Gets supported endpoint protocols.
		/// </summary>
		/// <returns>Set of supported endpoint protocols.</returns>
		private static HashSet<EndpointProtocol> GetSupportedEndpointProtocols()
		{
			return new HashSet<EndpointProtocol>(1)
			{
				EndpointProtocol.Http
			};
		}

		/// <summary>
		/// Creates empty protocol to endpoint map for provided <paramref name="supportedEndpointProtocols"/>.
		/// </summary>
		/// <param name="supportedEndpointProtocols">Supported endpoint protocols.</param>
		/// <returns>Empty protocol to endpoint map for provided <paramref name="supportedEndpointProtocols"/>.</returns>
		private static Dictionary<EndpointProtocol, List<EndpointResourceDescription>> CreateEmptyProtocolToEdnpointMap(HashSet<EndpointProtocol> supportedEndpointProtocols)
		{
			Dictionary<EndpointProtocol, List<EndpointResourceDescription>> endpointsByProtocol = new Dictionary<EndpointProtocol, List<EndpointResourceDescription>>(supportedEndpointProtocols.Count);
			foreach (EndpointProtocol supportedProtocol in supportedEndpointProtocols)
			{
				endpointsByProtocol[supportedProtocol] = new List<EndpointResourceDescription>();
			}

			return endpointsByProtocol;
		}
	}
}
