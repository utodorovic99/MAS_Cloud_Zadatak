using BookstoreAPI.Listeners.Controllers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace BookstoreAPI.Listeners.Http.Controllers
{
	/// <summary>
	/// Base implementation for HTTP controllers.
	/// </summary>
	internal abstract class HttpControllerBase : IHttpController
	{
		protected const int MaxWaitTimeoutMs = 30000;

		/// <summary>
		/// Initializes new instance of <see cref="HttpControllerBase"/>.
		/// </summary>
		/// <param name="controllerName">Name of controller.</param>
		public HttpControllerBase(string controllerName)
		{
			this.ControllerName = controllerName;
		}

		/// <inheritdoc/>
		public string ControllerName { get; protected set; }

		/// <inheritdoc/>
		public abstract IEnumerable<string> RequestsIdentifiers { get; }

		/// <inheritdoc/>
		public abstract void ProcessRequest(string requestIdentifier, HttpListenerContext context);

		/// <summary>
		/// Deserialize provided <paramref name="httpRequest"/> into a object of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">Deserialized object type.</typeparam>
		/// <param name="httpRequest">HTTP request.</param>
		/// <returns>Object deserialized from <paramref name="httpRequest"/>.</returns>
		protected T DeserializeHttpRequest<T>(HttpListenerRequest httpRequest)
		{
			string json;
			using (var reader = new StreamReader(httpRequest.InputStream, httpRequest.ContentEncoding))
			{
				json = reader.ReadToEnd();
			}

			return JsonSerializer.Deserialize<T>(json);
		}

		/// <summary>
		/// This method schedules auto-cancellation which will abort following awaits after configured time exceeds.
		/// </summary>
		/// <param name="cts">Resulting cancellation token source.</param>
		protected void ScheduleTimedAutoCancellation(out CancellationTokenSource cts)
		{
			cts = new CancellationTokenSource();
			cts.CancelAfter(MaxWaitTimeoutMs);
		}

		/// <summary>
		/// Submits <paramref name="context"/> as ailed with <paramref name="failureCode"/>.
		/// </summary>
		/// <<param name="context">Listener context.</param>
		/// <param name="failureCode">Failure code.</param>
		protected void SubmitResponseAsFailure(HttpListenerContext context, HttpStatusCode failureCode)
		{
			var response = context.Response;
			response.StatusCode = (int)failureCode;
			response.OutputStream.Close();
		}
	}
}
