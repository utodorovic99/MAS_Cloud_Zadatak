using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace ValidationServiceContract.Contract
{
	/// <summary>
	/// Contract for validation service.
	/// </summary>
	public interface IValidationServiceContract : IService
	{
		/// <summary>
		/// Tries to execute <paramref name="purchaseRequest"/> in case it is valid.
		/// </summary>
		/// <param name="purchaseRequest">Request to execute.</param>
		/// <returns>Response on <paramref name="purchaseRequest"/>.</returns>
		Task<PurchaseResponse> TryExecutePurchase(PurchaseRequest purchaseRequest);
	}
}
