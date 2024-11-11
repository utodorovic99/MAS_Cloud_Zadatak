using BookstoreDesktopClient.Resources;
using BookstoreDesktopClient.ViewModel;
using System.Windows;

namespace BookstoreDesktopClient
{
	/// <summary>
	/// Class encapsulating logic for creation of message boxes.
	/// </summary>
	internal static class MessageBoxHelper
	{
		/// <summary>
		/// Creates message box with data from <paramref name="purchaseResponse"/>.
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="purchaseResponse">Purchase response.</param>
		public static void DisplayFor(Window parentWindow, PurchaseResponseWrapper purchaseResponse)
		{
			if (purchaseResponse.Status)
			{
				DisplayDialogForSucessfullOperation(parentWindow, purchaseResponse);
			}
			else
			{
				DisplayDialogForFailedOperation(parentWindow, purchaseResponse);
			}
		}

		/// <summary>
		/// Creates message box with data from <paramref name="purchaseResponse"/> indicating successful operation.
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="purchaseResponse">Purchase response.</param>
		private static void DisplayDialogForSucessfullOperation(Window parentWindow, PurchaseResponseWrapper purchaseResponse)
		{
			MessageBoxImage image = MessageBoxImage.Information;
			string caption = BookstoreResources.ResultDialogContent_SUCCESS;
			string messageBoxText = string.Format(BookstoreResources.BookPurchaseResult_SUCCEEDED, purchaseResponse.Title);

			parentWindow.Dispatcher.Invoke(() =>
			{
				MessageBox.Show(parentWindow, messageBoxText, caption, MessageBoxButton.OKCancel, image);
			});
		}

		/// <summary>
		/// Creates message box with data from <paramref name="purchaseResponse"/> indicating failed operation.
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="purchaseResponse">Purchase response.</param>
		private static void DisplayDialogForFailedOperation(Window parentWindow, PurchaseResponseWrapper purchaseResponse)
		{
			MessageBoxImage image = MessageBoxImage.Warning;
			string caption = BookstoreResources.ResultDialogContent_FAILURE;
			string messageBoxText = string.Format(BookstoreResources.BookPurchaseResult_FAILED, purchaseResponse.Title, purchaseResponse.Message);

			parentWindow.Dispatcher.Invoke(() =>
			{
				MessageBox.Show(parentWindow, messageBoxText, caption, MessageBoxButton.OKCancel, image);
			});
		}
	}
}
