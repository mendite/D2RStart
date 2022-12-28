using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace D2RStart
{
    internal interface IRelayCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    internal class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {
        }
    }

    internal class RelayCommand<T> : IRelayCommand
    {
        private readonly Action<T> _executeHandler;
        private readonly Predicate<T> _canExecuteHandler;

        public RelayCommand(Action<T> execute) : this(execute, null)
        { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("Execute cannot be null");
            _executeHandler = execute;
            _canExecuteHandler = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(T parameter)
        {
            _executeHandler(parameter);
        }

        public void Execute(object parameter)
        {
            this.Execute((T)parameter);
        }

        public bool CanExecute(T parameter)
        {
            if (_canExecuteHandler == null)
                return true;

            return _canExecuteHandler(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null && !typeof(T).IsAssignableFrom(parameter.GetType()))
                return false;

            return this.CanExecute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
