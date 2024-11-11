using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BookstoreDesktopClient.Helpers
{
	/// <summary>
	/// Wrapper around <see cref="ObservableCollection{T}"/> allowing lighter approach to
	/// notifying changes.
	/// </summary>
	/// <typeparam name="T">Type of elements stored inside collection.</typeparam>
	internal sealed class FastObservableCollection<T> : ObservableCollection<T>
	{
		private bool suppressNotifications = false;

		/// <summary>
		/// Initializes new instance of see <see cref="FastObservableCollection{T}"/>
		/// </summary>
		public FastObservableCollection()
			: base()
		{
		}

		/// <summary>
		/// Initializes new instance of see <see cref="FastObservableCollection{T}"/>
		/// </summary>
		/// <param name="items">Initial content of collection.</param>
		public FastObservableCollection(IEnumerable<T> items)
			: base(items)
		{
		}

		/// <summary>
		/// Updates content of list to <see cref="newItems"/>
		/// </summary>
		/// <param name="newItems">New items present in collection.</param>
		public void Update(IEnumerable<T> newItems)
		{
			suppressNotifications = true;

			Items.Clear();
			foreach (T item in newItems)
			{
				Items.Add(item);
			}

			suppressNotifications = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		/// <inheritdoc/>
		/// <remarks>
		/// Notifications are suppressed based on <see cref="suppressNotifications"/>.
		/// </remarks>
		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (suppressNotifications)
			{
				return;
			}

			base.OnPropertyChanged(e);
		}
	}
}
