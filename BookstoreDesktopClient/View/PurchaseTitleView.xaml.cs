using BookstoreDesktopClient.ViewModel;
using System.Windows.Controls;

namespace BookstoreDesktopClient.View
{
	/// <summary>
	/// Interaction logic for PurchaseTitleView.xaml
	/// </summary>
	public partial class PurchaseTitleView : UserControl
	{
		/// <summary>
		/// Initializes new instance of <see cref="PurchaseTitleView"/>
		/// </summary>
		public PurchaseTitleView()
		{
			this.DataContext = new PurchaseTitleViewModel();
			InitializeComponent();
		}
	}
}
