using BookstoreAPI.Listeners.Controllers;
using System.Net;

namespace BookstoreAPI.Listeners.Http
{
	/// <summary>
	/// Processor for HTTP requests.
	/// </summary>
	internal sealed class HttpRequestProcessor : BaseRequestProcessor<HttpListenerContext>
	{
		private readonly ControllerResolver controllerResolver;

		/// <summary>
		/// Initializes new instance of <see cref="HttpRequestProcessor"/>
		/// </summary>
		/// <param name="controllerResolver">Resolver for HTTP controllers.</param>
		public HttpRequestProcessor(ControllerResolver controllerResolver)
		{
			this.controllerResolver = controllerResolver;
		}

		/// <summary>
		/// Handles HTTP request.
		/// </summary>
		/// <param name="requestContext">HTTP request context.</param>
		protected override void HandleRequest(HttpListenerContext requestContext)
		{
			HttpListenerRequest request = requestContext.Request;
			if (controllerResolver.TryResolve(request, out string requestId, out IHttpController controller))
			{
				controller.ProcessRequest(requestId, requestContext);
			}
		}
	}
}
