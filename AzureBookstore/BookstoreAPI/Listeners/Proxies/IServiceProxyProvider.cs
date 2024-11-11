using Microsoft.ServiceFabric.Services.Remoting;
using System;

namespace BookstoreAPI.Listeners
{
	/// <summary>
	/// Contract for retrieving service proxies.
	/// </summary>
	internal interface IServiceProxyProvider
	{
		/// <summary>
		/// Gets proxy registered for <paramref name="contractType"/>.
		/// </summary>
		/// <param name="contractType">Type of service contract.</param>
		/// <returns>Service proxy of type <paramref name="contractType"/>.</returns>
		IService GetProxyFor(Type contractType);
	}
}