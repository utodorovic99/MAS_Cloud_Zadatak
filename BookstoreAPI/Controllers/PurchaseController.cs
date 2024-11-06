using Microsoft.AspNetCore.Mvc;
using PurchaseDataModel;
using ValidationDataModel;

namespace BookstoreAPI.Controllers
{
	/// <summary>
	/// Controller responsible for accepting book purchase requests.
	/// </summary>
	[ApiController]
	[Route("[controller]")]
	public class PurchaseController : ControllerBase
	{
		private readonly ILogger<PurchaseController> logger;
		private readonly IValidationServiceProxy validationServiceProxy;

		/// <summary>
		/// Initializes new instance of <see cref=PurchaseController"/>
		/// </summary>
		/// <param name="logger">Logger.</param>
		public PurchaseController(ILogger<PurchaseController> logger)
		{
			this.logger = logger;
			validationServiceProxy = new ValidationServiceProxy();
		}

		/// <summary>
		/// HTTP Post request handler for purchase of book titles.
		/// </summary>
		/// <param name="purchase">Title purchase.</param>
		/// <returns>Purchase result.</returns>
		[HttpPost("PurchaseTitle")]
		public async Task<PurchaseResponse> Post(PurchaseRequest purchase)
		{
			PurchaseResponse purchaseResponse = new PurchaseResponse();

			PurchaseValidationResponse validationResponse = await validationServiceProxy.ValidatePurchaseRequest(purchase);
			if (!validationResponse.ValidityStatus.Equals(PurchaseValidityStatus.Valid))
			{
				purchaseResponse.Status = StatusHelper.ToResponseStatus(validationResponse.ValidityStatus);
			}

			purchaseResponse.Status = PurchaseResponseStatus.Success;
			return purchaseResponse;
		}
	}
}
