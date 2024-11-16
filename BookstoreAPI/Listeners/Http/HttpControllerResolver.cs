using BookstoreAPI.Listeners.Controllers;
using System.Collections.Generic;
using System.Net;

namespace BookstoreAPI.Listeners.Http
{
	/// <summary>
	/// Class used for resolving controller responsible for each request type.
	/// </summary>
	internal sealed class ControllerResolver
	{
		private const string SlashStr = "/";

		private Dictionary<string, IHttpController> controllersByRequestIdentifier;

		/// <summary>
		/// Initializes new instance of <see cref="ControllerResolver"/>
		/// </summary>
		public ControllerResolver()
		{
			controllersByRequestIdentifier = new Dictionary<string, IHttpController>();
		}

		/// <summary>
		/// Registers <paramref name="controller"/>.
		/// </summary>
		/// <param name="controller">HTTP controller to be registered.</param>
		public void RegisterController(IHttpController controller)
		{
			foreach (string requestIdentifier in controller.RequestsIdentifiers)
			{
				controllersByRequestIdentifier[requestIdentifier] = controller;
			}
		}

		/// <summary>
		/// Tries to resolve which type of <see cref="IHttpController"/> is responsible for <paramref name="request"/>.
		/// </summary>
		/// <param name="request">HTTP request.</param>
		/// <param name="requestId">Request identifier which uniquely identifies type of request.</param>
		/// <param name="controller">Resulting controller responsible for <paramref name="controller"/>.</param>
		public bool TryResolve(HttpListenerRequest request, out string requestId, out IHttpController controller)
		{
			controller = null;
			requestId = request.Url.LocalPath;

			if (!IsSlashTerminated(requestId))
			{
				requestId = $"{requestId}/";
			}

			return controllersByRequestIdentifier.TryGetValue(requestId, out controller);
		}

		/// <summary>
		/// Gets request types which can be resolved.
		/// </summary>
		public IEnumerable<string> ResolvableRequestTypes
		{
			get
			{
				return controllersByRequestIdentifier.Keys;
			}
		}

		/// <summary>
		/// Checks whether <paramref name="requestId"/> ends with '/' character.
		/// </summary>
		/// <param name="requestId">Request id to check.</param>
		/// <returns><c>True</c> if <paramref name="requestId"/> ends with '/' character; otherwise returns <c>false</c>.</returns>
		private bool IsSlashTerminated(string requestId)
		{
			return requestId.EndsWith(SlashStr);
		}
	}
}
