using Microsoft.ServiceFabric.Services.Remoting;

namespace CommunicationsSDK.Proxies
{
	/// <summary>
	/// Contract for retrieving service proxies.
	/// </summary>
	public interface IServiceProxyProvider
	{
		/// <summary>
		/// Gets proxy registered for <paramref name="contractType"/>.
		/// </summary>
		/// <typeparam name="T">Type of proxy.</typeparam>
		/// <returns>Service proxy of type <paramref name="contractType"/>.</returns>
		T GetProxyFor<T>() where T : IService;
	}
}