using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;
using System.Fabric.Description;
using ValidationService.Helpers;
using ValidationService.ServiceListeners;

namespace ValidationService
{
	/// <summary>
	/// An instance of this class is created for each service instance by the Service Fabric runtime.
	/// </summary>
	internal sealed class ValidationService : StatelessService
	{
		/// <summary>
		/// Initializes new instance of <see cref="ValidationService"/>.
		/// </summary>
		/// <param name="context">Service context.</param>
		public ValidationService(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Gets service name used for its exposure through API.
		/// </summary>
		public static string APIName { get; } = "ValidationService";

		/// <summary>
		/// Creates HTTP listeners for this service replica to handle client or user requests.
		/// </summary>
		/// <returns>A collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			StatelessServiceContext serviceContext = this.Context;
			Dictionary<EndpointProtocol, List<EndpointResourceDescription>> endpointsByProtocol = ServiceManifestHelper.GetValidationServiceEndpointsByProtocol(serviceContext);

			return ValidationServiceListenerFactory.CreateListeners(endpointsByProtocol);
		}
	}
}
