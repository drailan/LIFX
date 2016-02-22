using System;
using System.Windows.Input;

namespace LIFXGui.Commands
{
	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		private Action _action;
		private Func<bool> _predicate;

		public RelayCommand(Action methodToExecute, Func<bool> predicate)
		{
			_action = methodToExecute;
			_predicate = predicate;
		}

		public RelayCommand(Action methodToExecute)
			: this(methodToExecute, null)
		{
		}

		public bool CanExecute(object parameter)
		{
			return _predicate != null ? _predicate() : true;
		}
		public void Execute(object parameter)
		{
			_action.Invoke();
		}

		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}

}
