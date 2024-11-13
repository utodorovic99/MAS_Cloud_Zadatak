using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Diagnostics;
using System.Threading;

namespace BookstoreAPI
{
	/// <summary>
	/// Service program driver.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Gets service configuration.
		/// </summary>
		public static Configuration Configuration { get; private set; } = new Configuration();

		/// <summary>
		/// This is the entry point of the service host process.
		/// </summary>
		private static void Main()
		{
			try
			{
				Configuration.Initialize();

				ServiceRuntime.RegisterServiceAsync("BookstoreAPIType",
					context => new BookstoreAPI(context)).GetAwaiter().GetResult();

				ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(BookstoreAPI).Name);

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
