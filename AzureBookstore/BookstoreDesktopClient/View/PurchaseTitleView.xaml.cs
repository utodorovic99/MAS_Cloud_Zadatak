using BookstoreDesktopClient.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace BookstoreDesktopClient.View
{
	/// <summary>
	/// Interaction logic for PurchaseTitleView.xaml
	/// </summary>
	public partial class PurchaseTitleView : UserControl
	{
		private PurchaseTitleViewModel viewModel;

		/// <summary>
		/// Initializes new instance of <see cref="PurchaseTitleView"/>
		/// </summary>
		public PurchaseTitleView()
		{
			InitializeComponent();

			viewModel = new PurchaseTitleViewModel();
			this.DataContext = viewModel;
		}

		/// <summary>
		/// Safely destroys current instance while releasing all its resources.
		/// </summary>
		~PurchaseTitleView()
		{
			viewModel.Dispose();
		}

		/// <summary>
		/// Event handler for situation when user control is loaded.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event arguments.</param>
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel.ParentWindow = Window.GetWindow(this);
		}
	}
}
