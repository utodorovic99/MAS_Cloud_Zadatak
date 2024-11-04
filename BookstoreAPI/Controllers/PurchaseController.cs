using BookstoreDataModel;
using Microsoft.AspNetCore.Mvc;

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

		/// <summary>
		/// Initializes new instance of <see cref=PurchaseController"/>
		/// </summary>
		/// <param name="logger">Logger.</param>
		public PurchaseController(ILogger<PurchaseController> logger)
		{
			this.logger = logger;
		}

		/// <summary>
		/// HTTP Post request handler for purchase of book titles.
		/// </summary>
		/// <param name="purchase">Title purchase.</param>
		/// <returns>Purchase result.</returns>
		[HttpPost("PurchaseTitle")]
		public PurchaseResponse Post(PurchaseRequest purchase)
		{
			return new PurchaseResponse()
			{
				Status = PurchaseResponseStatus.Success,
			};
		}
	}
}
