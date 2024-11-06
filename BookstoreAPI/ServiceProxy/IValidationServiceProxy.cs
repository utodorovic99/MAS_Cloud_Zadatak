using PurchaseDataModel;
using ValidationDataModel;

namespace BookstoreAPI
{
	/// <summary>
	/// Proxy for validation service.
	/// </summary>
	internal interface IValidationServiceProxy : IDisposable
	{
		/// <summary>
		/// Validates purchase request.
		/// </summary>
		/// <param name="purchaseRequest">Purchase request to be validated.</param>
		/// <returns>Task with purchase validation response.</returns>
		Task<PurchaseValidationResponse> ValidatePurchaseRequest(PurchaseRequest purchaseRequest);
	}
}