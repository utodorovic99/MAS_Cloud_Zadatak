using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace TransactionCoordinatingServiceContract.Contract
{
	/// <summary>
	/// Contract for transaction coordinating service.
	/// </summary>
	public interface ITransactionCoordinatingServiceContract : IService
	{
		/// <summary>
		/// Executes <paramref name="purchaseRequest"/> while coordinating its executions
		/// throughout all interested parties.
		/// </summary>
		/// <param name="purchaseRequest">Request to execute.</param>
		/// <returns>Response on <paramref name="purchaseRequest"/>.</returns>
		Task<PurchaseResponse> ExecuteCoordinatedPurchase(PurchaseRequest purchaseRequest);
	}
}
