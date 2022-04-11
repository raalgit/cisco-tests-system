using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace KAF304TESTS.CiscoTestEditor
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute(object parameters)
        {
            _action();
        }

        public bool CanExecute(object parameters)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
