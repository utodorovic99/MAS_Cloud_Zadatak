using CommunicationsSDK.Http;
using PurchaseDataModel;
using ValidationDataModel;

namespace BookstoreAPI
{
	/// <summary>
	/// Represents service proxy for validation service.
	/// </summary>
	internal sealed class ValidationServiceProxy : IValidationServiceProxy
	{
		private const int MaxResponseTimeoutMs = 5000;
		private readonly HttpClient validationServiceHttpClient;

		/// <summary>
		/// Initializes new instance of <see cref="ValidationServiceProxy"/>
		/// </summary>
		public ValidationServiceProxy()
		{
			validationServiceHttpClient = new HttpClient()
				.WithBaseAddress(Program.Configuration.ValidationServiceUri)
				.WithRequestTimeout(MaxResponseTimeoutMs);
		}

		/// <inheritdoc/>
		public Task<PurchaseValidationResponse> ValidatePurchaseRequest(PurchaseRequest purchaseRequest)
		{
			HttpContent purchaseRequestHttpContent = JsonContent.Create(purchaseRequest);
			CancellationToken cancellationToken = new CancellationTokenSource(MaxResponseTimeoutMs).Token;
			string requestName = "ValidatePurchaseRequest";

			Task<PurchaseValidationResponse> validationTask = Task<PurchaseValidationResponse>.Factory.StartNew(async () =>
			{
				HttpResponseMessage httpResponse = await validationServiceHttpClient.PostAsync(requestName, purchaseRequestHttpContent, cancellationToken);
				PurchaseValidationResponse validationResult = await httpResponse.Content.ReadFromJsonAsync<PurchaseValidationResponse>(cancellationToken);

				return validationResult;
			}, cancellationToken);

			validationTask.ContinueWith(t =>
			{
				t.Exception?.Handle(ex =>
				{
					return true;
				});

				return CreateFailureTask();

			}, TaskContinuationOptions.OnlyOnFaulted);

			return validationTask;
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			validationServiceHttpClient.Dispose();
		}

		/// <summary>
		/// Creates task indicating that internal failure happened.
		/// </summary>
		/// <returns>Task indicating that internal failure happened.</returns>
		private Task<PurchaseValidationResponse> CreateFailureTask()
		{
			PurchaseValidationResponse internalErrorValidationResponse = new PurchaseValidationResponse();
			internalErrorValidationResponse.ValidityStatus = PurchaseValidityStatus.Invalid;

			return Task.FromResult(internalErrorValidationResponse);
		}
	}
}
