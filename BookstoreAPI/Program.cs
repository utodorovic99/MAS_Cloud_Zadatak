using Microsoft.ServiceFabric.Services.Runtime;
using System.Diagnostics;

namespace BookstoreAPI
{
	internal static class Program
	{
		public static Configuration Configuration { get; private set; } = new Configuration();

		/// <summary>
		/// This is the entry point of the service host process.
		/// </summary>
		private static void Main()
		{
			try
			{
				// The ServiceManifest.XML file defines one or more service type names.
				// Registering a service maps a service type name to a .NET type.
				// When Service Fabric creates an instance of this service type,
				// an instance of the class is created in this host process.

				Configuration.Initialize();

				ServiceRuntime.RegisterServiceAsync("BookstoreAPIType",
					context => new BookstoreAPI(context)).GetAwaiter().GetResult();

				ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(BookstoreAPI).Name);

				// Prevents this host process from terminating so services keeps running.
				Thread.Sleep(Timeout.Infinite);
			}
			catch (Exception e)
			{
				ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
				throw;
			}
		}
	}
}
