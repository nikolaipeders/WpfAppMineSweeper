using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfAppMineSweeper.Tools
{
    public class Command : ICommand
    {
        private Action<Object> actionToInvoke = null;

        public Command(Action<Object> actionToInvoke)
        {
            this.actionToInvoke = actionToInvoke;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            System.Diagnostics.Debug.WriteLine("Command was called");

            this.actionToInvoke(parameter);
        }
    }
}
