using System.Windows.Input;

namespace BookstoreDesktopClient.Commanding
{
	/// <summary>
	/// Class encapsulating command.
	/// </summary>
	internal sealed class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		/// <summary>
		/// Initializes new instance of <see cref="RelayCommand"/>.
		/// </summary>
		/// <param name="execute">Command execution.</param>
		/// <param name="canExecute">Command pre-execution validation.</param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		/// <inheritdoc/>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}

			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		/// <inheritdoc/>
		public bool CanExecute(object parameter)
		{
			return canExecute(parameter);
		}

		/// <inheritdoc/>
		public void Execute(object parameter)
		{
			execute(parameter);
		}
	}
}
