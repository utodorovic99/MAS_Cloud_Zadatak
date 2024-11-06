using BookstoreDesktopClient.Commanding;
using BookstoreDesktopClient.ServiceProxy;
using PurchaseDataModel;
using System.Windows;
using System.Windows.Input;
namespace BookstoreDesktopClient.ViewModel
{
	/// <summary>
	/// View model for controls related to title purchase.
	/// </summary>
	internal sealed class PurchaseTitleViewModel : IDisposable
	{
		private readonly IBookstoreServiceProxy bookstoreServiceProxy;
		private ICommand purchaseTitleCommand;

		/// <summary>
		/// Initializes new instance of <see cref="PurchaseTitleViewModel"/>.
		/// </summary>
		/// <param name="parentWindow">Parent window hosting views for current instance of <see cref="PurchaseTitleViewModel"/>.</param>
		public PurchaseTitleViewModel()
		{
			bookstoreServiceProxy = new BookstoreServiceProxy();
			SubscribeToEvents();
		}

		/// <summary>
		/// Gets or sets currently entered book title to purchase.
		/// </summary>
		public string EnteredTitleToPurchase { get; set; }

		/// <summary>
		/// Gets or sets parent window.
		/// </summary>
		public Window ParentWindow { get; set; }

		/// <summary>
		/// Gets command for purchasing book title.
		/// </summary>
		public ICommand PurchaseTitleCommand
		{
			get
			{
				return purchaseTitleCommand ?? (purchaseTitleCommand = new RelayCommand(param => ExecutePurchaseCommand(), param => CanExecutePurchaseCommand()));
			}
		}

		/// <summary>
		/// Subscribes current instance to all events of interests.
		/// </summary>
		public void SubscribeToEvents()
		{
			bookstoreServiceProxy.PurchaseReceivedEvent += PurchaseResponseReceivedEventHandler;
		}

		/// <summary>
		/// Unsubscribes current instance from all previously subscribed events of interest.
		/// </summary>

		public void UnsubscribeFromEvents()
		{
			bookstoreServiceProxy.PurchaseReceivedEvent -= PurchaseResponseReceivedEventHandler;
		}

		/// <summary>
		/// Handler for situations when purchase response is received.
		/// </summary>
		/// <param name="purchaseResponse"></param>
		public void PurchaseResponseReceivedEventHandler(PurchaseResponseWrapper purchaseResponse)
		{
			MessageBoxHelper.DisplayFor(ParentWindow, purchaseResponse);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			bookstoreServiceProxy.Dispose();
			UnsubscribeFromEvents();
		}

		/// <summary>
		/// Checks whether <see cref="PurchaseTitleCommand"/> can be executed.
		/// </summary>
		/// <returns><c>True</c> if <see cref="PurchaseTitleCommand"/> can be executed; otherwise returns <c>false</c>.</returns>
		private bool CanExecutePurchaseCommand()
		{
			return EnteredTitleToPurchase?.Length > 0;
		}

		/// <summary>
		/// Executes logic for <see cref="PurchaseTitleCommand"/>.
		/// </summary>
		private void ExecutePurchaseCommand()
		{
			PurchaseRequest purchaseRequest = new PurchaseRequest()
			{
				Title = EnteredTitleToPurchase,
			};

			bookstoreServiceProxy.SendPurchaseRequest(purchaseRequest);
		}

	}
}
