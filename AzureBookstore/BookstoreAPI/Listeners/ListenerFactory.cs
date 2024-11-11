﻿using BookstoreAPI.Listeners.Http;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Collections.Generic;
using System.Fabric.Description;

namespace BookstoreAPI.Listeners
{
	/// <summary>
	/// Class for creating <see cref="ServiceInstanceListener"/> type of objects.
	/// </summary>
	internal static class ListenerFactory
	{
		/// <summary>
		/// Creates <see cref="ServiceInstanceListener"/> for each one of <paramref name="endpoints"/>.
		/// </summary>
		/// <param name="endpoints">Set of endpoints for which listeners will be created.</param>
		/// <returns>Collection of <see cref="ServiceInstanceListener"/> for provided <paramref name="endpoints"/>.</returns>
		public static IEnumerable<ServiceInstanceListener> CreateFor(IEnumerable<EndpointResourceDescription> endpoints)
		{
			foreach (EndpointResourceDescription endpoint in endpoints)
			{
				switch (endpoint.Name)
				{
					case ExternalRequestHTTPListener.ManifestIdentifier:
						if (endpoint.Protocol.Equals(EndpointProtocol.Http)
							|| endpoint.Protocol.Equals(EndpointProtocol.Https))
						{
							yield return new ServiceInstanceListener(serviceContext => new ExternalRequestHTTPListener(endpoint));
						}

						break;
				}
			}
		}
	}
}
