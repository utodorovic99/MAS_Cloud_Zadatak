using BookstoreServiceContract.Contracts;
using BookstoreServiceContract.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using ValidationDataModel;

namespace BookstoreAPI.Listeners.Controllers
{
	/// <summary>
	/// Controller for handling requests related to bookstore titles.
	/// </summary>
	internal sealed class TitleController : IHttpController
	{
		private const string ControllerName = "Title";
		private const int MaxWaitTimeoutMs = 5000;

		private readonly Dictionary<string, Action<HttpListenerContext>> processingRoutinsByRequestIdentifier;
		private readonly IServiceProxyProvider proxyProvider;

		/// <summary>
		/// Initializes title controller.
		/// </summary>
		/// <param name="proxyProvider">Service proxy provider.</param>
		public TitleController(IServiceProxyProvider proxyProvider)
		{
			this.proxyProvider = proxyProvider;

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
		/// Processes request for getting all titles.
		/// </summary>
		/// <param name="context">Listener context.</param>
		private async void ProcessGetAllTitles(HttpListenerContext context)
		{
			try
			{
				IBookstoreServiceContract serviceProxy = (IBookstoreServiceContract)proxyProvider.GetProxyFor(typeof(IBookstoreServiceContract));

				SheduleWaitCancellation(out CancellationTokenSource cts);
				IEnumerable<BookstoreTitle> allTitles = await serviceProxy.GetAllTitles();

				JsonContent getAllTitlesResponseContent = JsonContent.Create(allTitles);
				byte[] validationResponseContentRaw = getAllTitlesResponseContent.ReadAsByteArrayAsync().Result;

				var response = context.Response;
				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/json";

				response.OutputStream.Write(validationResponseContentRaw, 0, validationResponseContentRaw.Length);
				response.OutputStream.Close();
			}
			catch (Exception e)
			{
				SubmitResposeAsFailure(context, HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Processes validation request.
		/// </summary>
		/// <param name="context">Listener context.</param>
		private void ProcessPurchaseTitle(HttpListenerContext context)
		{
			try
			{
				var response = context.Response;

				PurchaseValidationResponse validationResponse = new PurchaseValidationResponse()
				{
					ValidityStatus = PurchaseValidityStatus.Valid,
				};

				JsonContent validationResponseContent = JsonContent.Create(validationResponse);
				byte[] validationResponseContentRaw = validationResponseContent.ReadAsByteArrayAsync().Result;

				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/json";

				response.OutputStream.Write(validationResponseContentRaw, 0, 0);
				response.OutputStream.Close();
			}
			catch (Exception e)
			{
				SubmitResposeAsFailure(context, HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// This method schedules auto-cancellation which will abort following awaits after configured time exceeds.
		/// </summary>
		/// <param name="cts">Resulting cancellation token source.</param>
		private void SheduleWaitCancellation(out CancellationTokenSource cts)
		{
			cts = new CancellationTokenSource();
			cts.CancelAfter(MaxWaitTimeoutMs);
		}

		/// <summary>
		/// Submits <paramref name="context"/> as ailed with <paramref name="failureCode"/>.
		/// </summary>
		/// <<param name="context">Listener context.</param>
		/// <param name="failureCode">Failure code.</param>
		private void SubmitResposeAsFailure(HttpListenerContext context, HttpStatusCode failureCode)
		{
			var response = context.Response;
			response.StatusCode = (int)failureCode;
			response.OutputStream.Close();
		}
	}
}
