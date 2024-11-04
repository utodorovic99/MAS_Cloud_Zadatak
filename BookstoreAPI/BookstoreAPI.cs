using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace BookstoreAPI
{
	/// <summary>
	/// The FabricRuntime creates an instance of this class for each service type instance.
	/// </summary>
	internal sealed class BookstoreAPI : StatelessService
	{
		/// <summary>
		/// Creates new instance of <see cref="BookstoreAPI"/>
		/// </summary>
		/// <param name="context">Service context.</param>
		public BookstoreAPI(StatelessServiceContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Optional override to create listeners (like tcp, http) for this service instance.
		/// </summary>
		/// <returns>The collection of listeners.</returns>
		protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
		{
			return new ServiceInstanceListener[]
			{
				new ServiceInstanceListener(serviceContext =>
					new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
					{
						ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

						var builder = WebApplication.CreateBuilder();

						builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
						builder.WebHost
									.UseKestrel()
									.UseContentRoot(Directory.GetCurrentDirectory())
									.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
									.UseUrls(url);
						builder.Services.AddControllers();
						builder.Services.AddEndpointsApiExplorer();
						builder.Services.AddSwaggerGen();
						var app = builder.Build();
						if (app.Environment.IsDevelopment())
						{
							app.UseSwagger();
							app.UseSwaggerUI();
						}

						app.UseAuthorization();
						app.MapControllers();

						return app;

					}))
			};
		}
	}
}
