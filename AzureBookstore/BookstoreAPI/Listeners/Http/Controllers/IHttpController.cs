using System.Collections.Generic;
using System.Net;

namespace BookstoreAPI.Listeners.Controllers
{
	/// <summary>
	/// Contract for HTTP request controllers.
	/// </summary>
	internal interface IHttpController
	{
		/// <summary>
		/// Gets name of controller matching one from URI.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets identifiers handled by the controller.
		/// </summary>
		/// <remarks>
		/// Request identifier is consisted of name of controller and name of request itself formatted as {ControllerName}/{RequestName}
		/// </remarks>
		IEnumerable<string> RequestsIdentifiers { get; }

		/// <summary>
		/// Processes request encapsulated within <paramref name="context"/>.
		/// </summary>
		/// <param name="requestIdentifier">Request identifier.</param>
		/// <param name="context">Context containing request and future response data.</param>
		void ProcessRequest(string requestIdentifier, HttpListenerContext context);
	}
}