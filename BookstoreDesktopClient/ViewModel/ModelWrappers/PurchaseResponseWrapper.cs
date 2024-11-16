using Common.Enums;
using Common.Model;

namespace BookstoreDesktopClient.ViewModel
{
	/// <summary>
	/// Wrapper around <see cref="PurchaseResponse"/>
	/// </summary>
	public sealed class PurchaseResponseWrapper
	{
		/// <summary>
		/// Initializes new instance of <see cref="PurchaseResponseWrapper"/>.
		/// </summary>
		public PurchaseResponseWrapper()
		{
			Title = string.Empty;
			Status = false;
			Message = string.Empty;
		}

		/// <summary>
		/// Initializes new instance of <see cref="PurchaseResponseWrapper"/>.
		/// </summary>
		/// <param name="title">Book title whose purchase has been attempted.</param>
		/// <param name="response">Purchase response.</param>
		public PurchaseResponseWrapper(string title, PurchaseResponse response)
		{
			Title = title;
			Status = response.Status.Equals(PurchaseResponseStatus.Success);
			Message = response.Status.ToLocalizedString();
		}

		/// <summary>
		/// Gets or sets title of book.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets indicator whether request succeeded.
		/// </summary>
		public bool Status { get; set; }

		/// <summary>
		/// Gets or sets message related to the response.
		/// </summary>
		public string Message { get; set; }
	}
}
