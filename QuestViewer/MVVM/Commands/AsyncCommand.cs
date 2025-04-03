using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestViewer
{
    public class AsyncCommand<T> : ICommand<T>
    {
        readonly Func<T, Task> execute;
        readonly Func<bool> canExecute;

        public AsyncCommand(Func<T, Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public bool CanExecute(T parameter)
        {
            return canExecute == null || canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var task = execute((T)parameter);
            task.Track();

            if (task.Exception != null)
                throw task.Exception;
        }

        public void Execute(T parameter)
        {
            var task = execute(parameter);
            task.Track();

            if (task.Exception != null)
                throw task.Exception;
        }
    }

    public class AsyncCommand : ICommand
    {
        readonly Func<Task> execute;
        readonly Func<bool> canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var task = execute();
            task.Track();

            if (task.Exception != null)
                throw task.Exception;
        }
    }
}