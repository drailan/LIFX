using System;
using System.Windows.Input;

namespace LIFXGui.Commands
{
	class CommandBase : ICommand
	{
		private readonly Action _action;
		private Func<bool> _canExecute;

		public CommandBase(Action action)
		{
			_action = action;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute();
		}

		public ICommand CanExecute(Func<bool> predicate)
		{
			_canExecute = predicate;
			return this;
		}

		public void Execute(object parameter)
		{
			_action();
		}
	}
}
