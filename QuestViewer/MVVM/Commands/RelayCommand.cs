using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuestViewer
{
    public interface ICommand<in T> : ICommand
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }

    public class RelayCommand<T> : ICommand<T>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        readonly Action<T> execute;
        readonly Predicate<T> canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                return canExecute == null || canExecute((T)parameter);
            }
            catch (Exception ex)
            {
                logger.Error(ex.GetBaseException());
                throw;
            }
        }

        public bool CanExecute(T parameter)
        {
            try
            {
                return canExecute == null || canExecute(parameter);
            }
            catch (Exception ex)
            {
                logger.Error(ex.GetBaseException());
                throw;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        public void Execute(T parameter)
        {
            execute(parameter);
        }
    }

    public class RelayCommand : ICommand
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        readonly Action execute;
        readonly Func<bool> canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                return canExecute == null || canExecute();
            }
            catch (Exception ex)
            {
                logger.Error(ex.GetBaseException());
                throw;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}