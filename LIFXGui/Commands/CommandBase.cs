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
        public event EventHandler CanExecuteChanged;
        private readonly Action _action;

        public CommandBase(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
