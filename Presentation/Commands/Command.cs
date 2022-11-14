using System;
using System.Windows.Input;

namespace Presentation.Commands
{
    public class Command : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> execute, Predicate<object>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute is null || canExecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            execute(parameter!);
        }
    }
}
