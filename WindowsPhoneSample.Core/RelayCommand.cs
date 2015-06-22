using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WindowsPhoneSample.Core
{
    public sealed class RelayCommand : ICommand
    {
        readonly Action<object> execute;
        readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate = null)
        {
            Contract.AssertNotNull(executeAction, "executeAction");
            execute = executeAction;
            canExecute = canExecutePredicate;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return (canExecute == null) || canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged(EventArgs e)
        {
            var eventHandler = CanExecuteChanged;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}