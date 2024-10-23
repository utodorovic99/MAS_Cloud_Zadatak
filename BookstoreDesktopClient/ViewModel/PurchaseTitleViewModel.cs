using BookstoreDesktopClient.Commanding;

namespace BookstoreDesktopClient.ViewModel
{
	/// <summary>
	/// View model for controls related to title purchase.
	/// </summary>
	internal sealed class PurchaseTitleViewModel
	{
		private RelayCommand purchaseTitleCommand;

		/// <summary>
		/// Gets or sets currently entered book title to purchase.
		/// </summary>
		public string EnteredTitleToPurchase { get; set; }

		/// <summary>
		/// Gets command for purchasing book title.
		/// </summary>
		public RelayCommand PurchaseTitleCommand
		{
			get
			{
				return purchaseTitleCommand ?? (purchaseTitleCommand = new RelayCommand(param => ExecutePurchaseCommand(), param => CanExecutePurchaseCommand()));
			}
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
			//TODO: Send to the network
		}
	}
}
