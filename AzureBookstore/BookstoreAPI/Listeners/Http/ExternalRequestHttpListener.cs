using BookstoreAPI.Extensions;
using BookstoreAPI.Listeners.Controllers;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Fabric.Description;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreAPI.Listeners.Http
{
	/// <summary>
	/// Base implementation of HTTP listener.
	/// </summary>
	internal sealed class ExternalRequestHTTPListener : ICommunicationListener, IDisposable
	{
		public const string ManifestIdentifier = "ExternalHTTPListener";

		private readonly string listeningBaseUri;
		private readonly ControllerResolver controllerResolver;
		private HttpListener httpListener;
		private HttpRequestProcessor requestProcessor;

		/// <summary>
		/// Initializes new instance of <see cref="ExternalRequestHTTPListener"/>.
		/// </summary>
		/// <param name="endpoint">Endpoint on which listener will listen.</param>
		/// <param name="proxyProvider">Provider of service proxies.</param>
		public ExternalRequestHTTPListener(EndpointResourceDescription endpoint, IServiceProxyProvider proxyProvider)
		{
			listeningBaseUri = CreateListeningUriFromManifest(endpoint);

			controllerResolver = new ControllerResolver();
			RegisterRequestControllers(proxyProvider);

			requestProcessor = new HttpRequestProcessor(controllerResolver);
		}

		/// <inheritdoc/>
		public void Abort()
		{
			if (httpListener == null)
			{
				return;
			}

			httpListener.Abort();
			httpListener = null;

			requestProcessor.Stop();
		}

		/// <inheritdoc/>
		public Task CloseAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (httpListener != null)
			{
				try
				{
					httpListener.Stop();
					httpListener.Close();
					httpListener = null;

					requestProcessor.Stop();
				}
				catch
				{
					//Do nothing
				}
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task<string> OpenAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			httpListener = new HttpListener();

			foreach (string requestId in controllerResolver.ResolvableRequestTypes)
			{
				string requestIdSuffix = requestId.TrimStart('/');
				httpListener.Prefixes.Add($"{listeningBaseUri}{requestIdSuffix}");
			}

			httpListener.Start();
			requestProcessor.Start();
			StartWaitingForRequest();

			return Task.FromResult(listeningBaseUri);
		}

		/// <summary>
		/// Starts listening for requests.
		/// </summary>
		private void StartWaitingForRequest()
		{
			httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
		}

		/// <summary>
		/// Callback listening for requests.
		/// </summary>
		/// <param name="listeningResult">Listening result.</param>
		/// <remarks>When request is received, waiting for new request is started automatically.</remarks>
		private void ListenerCallback(IAsyncResult listeningResult)
		{
			if (httpListener == null)
			{
				//Disposing guard.
				return;
			}

			if (httpListener.IsListening)
			{
				HttpListenerContext context = httpListener.EndGetContext(listeningResult);
				requestProcessor.EnqueueForProcessing(context);
			}

			StartWaitingForRequest();
		}

		/// <summary>
		/// Registers request controllers.
		/// </summary>
		/// <param name="proxyProvider">Provider of service proxies.</param>
		private void RegisterRequestControllers(IServiceProxyProvider proxyProvider)
		{
			controllerResolver.RegisterController(new TitleController(proxyProvider));
		}

		/// <summary>
		/// Creates listening URI for <paramref name="endpoint"/>.
		/// </summary>
		/// <param name="endpoint">Service endpoint.</param>
		private string CreateListeningUriFromManifest(EndpointResourceDescription endpoint)
		{
			//Note: It is important to use '+' instead of actual IP address.
			//Actual IP address will be resolved through AzureFabric infrastructure.
			return $"{endpoint.Protocol.FormattedString()}://+:{endpoint.Port}/";
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			this.Abort();
		}
	}
}
