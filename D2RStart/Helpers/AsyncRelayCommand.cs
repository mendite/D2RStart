using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace D2RStart
{
    internal interface IAsyncRelayCommand : ICommand
    {
        bool IsExecuting { get; }
        ICommand CancelCommand { get; }
    }

    internal class AsyncRelayCommand<TParameter> : NotifyPropertyChangedBase, IAsyncRelayCommand
    {
        #region Private fields
        private readonly Action<CancellationToken, TParameter> executeHandler;
        private readonly Predicate<TParameter> canExecuteHandler;
        #endregion

        #region Constructor
        public AsyncRelayCommand(Action<CancellationToken, TParameter> executeHandler, Predicate<TParameter> canExecuteHandler)
        {
            this.executeHandler = executeHandler;
            this.canExecuteHandler = canExecuteHandler;
            CancelCommand = new CancelAsyncCommand();
        }
        #endregion

        #region Public events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Public properties
        private bool isExecuting = false;
        public bool IsExecuting
        {
            get { return isExecuting; }
            protected set { base.OnPropertyChanged<bool>(ref isExecuting, value); }
        }

        private CancelAsyncCommand cancelCommand;
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            protected set { base.OnPropertyChanged<CancelAsyncCommand>(ref cancelCommand, value as CancelAsyncCommand); }
        }
        #endregion

        #region Public methods
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Interface implementations
        async void ICommand.Execute(object parameter)
        {
            if (IsExecuting)
                throw new InvalidOperationException($"Execution of {nameof(AsyncRelayCommand<TParameter>)} is not allowed while already executing.");

            await ExecuteAsync(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (IsExecuting)
                return false;

            if (canExecuteHandler == null)
                return true;

            return canExecuteHandler((TParameter)parameter);
        }
        #endregion

        #region Private methods
        private async Task ExecuteAsync(object parameter)
        {
            if (IsExecuting)
                return;

            IsExecuting = true;
            cancelCommand.NotifyCommandStarting();

            await Task.Factory.StartNew(() => {
                try
                {
                    executeHandler(cancelCommand.CancellationToken, (TParameter)parameter);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    IsExecuting = false;
                    Application.Current.Dispatcher.Invoke(new Action(() => { cancelCommand.NotifyCommandFinished(); }));                    
                }
            });
        }
        #endregion

        #region Private Classes
        private sealed class CancelAsyncCommand : NotifyPropertyChangedBase, ICommand
        {
            #region Private fields
            private object cancellationTokenSourceLockObject = new object();
            private CancellationTokenSource cancellationTokenSource;
            #endregion
                        
            #region Public events
            public event EventHandler CanExecuteChanged;
            #endregion

            #region Public properties
            private bool commandIsExecuting = false;
            public bool CommandIsExecuting
            {
                get { return commandIsExecuting; }
                private set 
                {
                    base.OnPropertyChanged<bool>(ref commandIsExecuting, value);
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }

            public CancellationToken CancellationToken
            {
                get
                {
                    lock (cancellationTokenSourceLockObject)
                        return cancellationTokenSource.Token;
                }
            }
            #endregion

            #region Public methods
            public void NotifyCommandStarting()
            {
                if (CommandIsExecuting)
                    return;

                lock (cancellationTokenSourceLockObject)
                    cancellationTokenSource = new CancellationTokenSource();

                CommandIsExecuting = true;
            }
            public void NotifyCommandFinished()
            {
                CommandIsExecuting = false;
            }
            #endregion

            #region Interface implementations
            bool ICommand.CanExecute(object parameter)
            {
                return CommandIsExecuting && !CancellationToken.IsCancellationRequested;
            }
            void ICommand.Execute(object parameter)
            {
                lock (cancellationTokenSourceLockObject)
                    cancellationTokenSource.Cancel();

                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
            #endregion
        }
        #endregion
    }
}
