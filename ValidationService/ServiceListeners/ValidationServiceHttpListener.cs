using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Fabric;
using System.Fabric.Description;
using System.Net;
using System.Net.Http.Json;
using ValidationDataModel;

namespace ValidationService
{
	/// <summary>
	/// Listener for validation requests.
	/// </summary>
	internal sealed class ValidationServiceHttpListener : ICommunicationListener
	{
		private readonly string listeningBaseUri;
		private HttpListener httpListener;

		/// <summary>
		/// Initializes new instance of <see cref="ValidationServiceHttpListener"/>.
		/// </summary>
		/// <param name="endpoint">Endpoint on which listener will listen.</param>
		public ValidationServiceHttpListener(EndpointResourceDescription endpoint)
		{
			listeningBaseUri = ResolveListeningUriFromManifest(endpoint);
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
		}

		/// <inheritdoc/>
		public Task CloseAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			//NOTE: Operations are primitive so they will be executed synchronously and cancellation token will be ignored.
			if (httpListener != null)
			{
				try
				{
					httpListener.Stop();
					httpListener.Close();
					httpListener = null;
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

			//NOTE: Operations are primitive so they will be executed synchronously and cancellation token will be ignored.
			string requestName = "ValidatePurchaseRequest";
			httpListener = new HttpListener();
			httpListener.Prefixes.Add($"{listeningBaseUri}/{requestName}");
			httpListener.Start();
			StartWaitingForRequest();

			return Task.FromResult(listeningBaseUri);
		}

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
			if (!httpListener.IsListening)
			{
				return;
			}

			var context = httpListener.EndGetContext(listeningResult);
			var request = context.Request;

			//Validate bookstore

			//Validate bank

			//Send response
			var response = context.Response;
			response.StatusCode = (int)HttpStatusCode.OK;
			response.ContentType = "application/json";

			PurchaseValidationResponse validationResponse = new PurchaseValidationResponse()
			{
				ValidityStatus = PurchaseValidityStatus.Valid,
			};

			JsonContent validationResponseContent = JsonContent.Create(validationResponse);
			byte[] validationResponseContentRaw = validationResponseContent.ReadAsByteArrayAsync().Result;

			response.OutputStream.Write(validationResponseContentRaw, 0, 0);
			response.OutputStream.Close();

			StartWaitingForRequest();
		}

		/// <summary>
		/// Resolves actual listening URI for <paramref name="endpoint"/>.
		/// </summary>
		/// <param name="endpoint">Service endpoint.</param>
		/// <returns>String representing actual address on which service listener will listen.</returns>
		private string ResolveListeningUriFromManifest(EndpointResourceDescription endpoint)
		{
			string uriPrefix = $"{endpoint.Protocol}://+:{endpoint.Port}/{ValidationService.APIName}";
			return uriPrefix.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
		}
	}
}
