using BookstoreServiceContracts.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using ValidationDataModel;

namespace BookstoreAPI.Listeners.Controllers
{
	/// <summary>
	/// Controller for handling requests related to bookstore titles.
	/// </summary>
	internal sealed class TitleController : IHttpController
	{
		private const string ControllerName = "Title";
		private static readonly Dictionary<string, Action<HttpListenerContext>> processingRoutinsByRequestIdentifier;

		/// <summary>
		/// Static initialization of <see cref="TitleController"/>
		/// </summary>
		static TitleController()
		{
			processingRoutinsByRequestIdentifier = new Dictionary<string, Action<HttpListenerContext>>(2);
			processingRoutinsByRequestIdentifier[$"/{ControllerName}/PurchaseTitle/"] = ProcessPurchaseTitle;
			processingRoutinsByRequestIdentifier[$"/{ControllerName}/GetAll/"] = ProcessGetAllTitles;
		}

		/// <inheritdoc/>
		public string Name
		{
			get
			{
				return ControllerName;
			}
		}

		/// <inheritdoc/
		public IEnumerable<string> RequestsIdentifiers
		{
			get
			{
				return processingRoutinsByRequestIdentifier.Keys;
			}
		}

		/// <inheritdoc/>
		public void ProcessRequest(string requestIdentifier, HttpListenerContext context)
		{
			if (processingRoutinsByRequestIdentifier.TryGetValue(requestIdentifier, out Action<HttpListenerContext> processingRoutine))
			{
				processingRoutine.Invoke(context);
			}
		}

		/// <summary>
		/// Processes get all titles request.
		/// </summary>
		/// <param name="context">Listener context.</param>
		private static void ProcessGetAllTitles(HttpListenerContext context)
		{
			ProcessRequestSafely(context, (ctx) =>
			{
				var response = context.Response;
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/json";

				//TODO: Retrieve from internal service
				IEnumerable<BookstoreTitle> allTitles = new List<BookstoreTitle>(2)
			{
				new BookstoreTitle("Jezeva Kucica, Branko Copic"),
				new BookstoreTitle("Luca Mikrokozma, Petar Petrovic Njegos"),
			};

				JsonContent getAllTitlesResponseContent = JsonContent.Create(allTitles);
				byte[] validationResponseContentRaw = getAllTitlesResponseContent.ReadAsByteArrayAsync().Result;

				response.OutputStream.Write(validationResponseContentRaw, 0, validationResponseContentRaw.Length);
				response.OutputStream.Close();
			});
		}

		/// <summary>
		/// Processes validation request.
		/// </summary>
		/// <param name="context">Listener context.</param>
		private static void ProcessPurchaseTitle(HttpListenerContext context)
		{
			ProcessRequestSafely(context, (ctx) =>
			{
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
			});
		}

		/// <summary>
		/// Processes request safely.
		/// </summary>
		/// <param name="context">Listener context.</param>
		/// <param name="handler">Handling routine.</param>
		private static void ProcessRequestSafely(HttpListenerContext context, Action<HttpListenerContext> handler)
		{
			try
			{
				handler.Invoke(context);
			}
			catch
			{
				var response = context.Response;
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}
		}
	}
}
