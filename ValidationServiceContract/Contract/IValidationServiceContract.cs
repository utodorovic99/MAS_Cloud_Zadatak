using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;
using ValidationDataContract;

namespace ValidationServiceContract.Contract
{
	/// <summary>
	/// Contract for validation service.
	/// </summary>
	public interface IValidationServiceContract : IService
	{
		/// <summary>
		/// Validates <paramref name="purchaseRequest"/>.
		/// </summary>
		/// <param name="purchaseRequest">Request to validate.</param>
		/// <returns>Validity status of <paramref name="purchaseRequest"/>.</returns>
		Task<PurchaseValidityStatus> ValidatePurchaseRequest(PurchaseRequest purchaseRequest);
	}
}
