using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace KeyboardTrening.ViewModel
{
    class ActionCommand : ICommand
    {
        private Action<object> execute;
        private bool canExecute;

        public ActionCommand(Action<object> execute, bool canExecute = true)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;
        

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
        public bool CanExecute(object parameter)
        {
            return this.canExecute;
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
