using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public void CanExecute(Func<bool> predicate)
		{
			_canExecute = predicate;
		}

		public void Execute(object parameter)
		{
			_action();
		}
	}
}
