using BookstoreAPI.Listeners.Http.Controllers;
using BookstoreServiceContract.Contracts;
using Common.Model;
using CommunicationsSDK.Proxies;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using ValidationServiceContract.Contract;

namespace BookstoreAPI.Listeners.Controllers
{
	/// <summary>
	/// Controller for handling requests related to bookstore titles.
	/// </summary>
	internal sealed class TitleController : HttpControllerBase
	{
		private readonly Dictionary<string, Action<HttpListenerContext>> processingRoutinsByRequestIdentifier;
		private readonly IServiceProxyProvider proxyProvider;

		/// <summary>
		/// Initializes title controller.
		/// </summary>
		/// <param name="proxyProvider">Service proxy provider.</param>
		public TitleController(IServiceProxyProvider proxyProvider)
			: base(controllerName: "Title")
		{
			this.proxyProvider = proxyProvider;

			processingRoutinsByRequestIdentifier = new Dictionary<string, Action<HttpListenerContext>>(2);
			processingRoutinsByRequestIdentifier[$"/{ControllerName}/PurchaseTitle/"] = ProcessPurchaseTitle;
			processingRoutinsByRequestIdentifier[$"/{ControllerName}/GetAll/"] = ProcessGetAllTitles;
		}

		/// <inheritdoc/
		public override IEnumerable<string> RequestsIdentifiers
		{
			get
			{
				return processingRoutinsByRequestIdentifier.Keys;
			}
		}

		/// <inheritdoc/>
		public override void ProcessRequest(string requestIdentifier, HttpListenerContext context)
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
				ScheduleTimedAutoCancellation(out CancellationTokenSource cts);

				IBookstoreServiceContract serviceProxy = proxyProvider.GetProxyFor<IBookstoreServiceContract>();
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
				SubmitResponseAsFailure(context, HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Processes validation request.
		/// </summary>
		/// <param name="context">Listener context.</param>
		private async void ProcessPurchaseTitle(HttpListenerContext context)
		{
			try
			{
				PurchaseRequest request = DeserializeHttpRequest<PurchaseRequest>(context.Request);

				ScheduleTimedAutoCancellation(out CancellationTokenSource cts);
				IValidationServiceContract serviceProxy = proxyProvider.GetProxyFor<IValidationServiceContract>();
				PurchaseResponse purchaseResponse = await serviceProxy.TryExecutePurchase(request);

				HttpListenerResponse response = context.Response;

				JsonContent validationResponseContent = JsonContent.Create(purchaseResponse);
				byte[] validationResponseContentRaw = validationResponseContent.ReadAsByteArrayAsync().Result;

				response.StatusCode = (int)HttpStatusCode.OK;
				response.ContentType = "application/json";

				response.OutputStream.Write(validationResponseContentRaw, 0, validationResponseContentRaw.Length);
				response.OutputStream.Close();
			}
			catch (Exception e)
			{
				SubmitResponseAsFailure(context, HttpStatusCode.InternalServerError);
			}
		}
	}
}
