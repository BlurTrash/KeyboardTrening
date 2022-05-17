using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace KeyboardTrening.ViewModel
{
    
    class KeyCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> _action;

        public KeyCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                var castParameter = (T)Convert.ChangeType(parameter, typeof(T));
                _action(castParameter);
            }
        }
    }
}
