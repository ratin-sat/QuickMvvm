using System;
using System.Windows.Input;

namespace QuickMvvm
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> execute;

        private readonly Func<T, bool> canExecute;

        private static bool CanExcute(T parameter) => true;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentException(nameof(execute));
            this.canExecute = canExecute ?? CanExcute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute(TranslateParameter(parameter));
        }

        public void Execute(object parameter)
        {
            this.execute(TranslateParameter(parameter));
        }

        private T TranslateParameter(object parameter)
        {
            var value = default(T);
            if (parameter != null && typeof(T).IsEnum)
            {
                value = (T)Enum.Parse(typeof(T), (string)parameter);
            }
            else
            {
                value = (T)parameter;
            }

            return value;
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
            : base(obj => execute(), (canExecute == null) ? null : new Func<object, bool>(obj => canExecute()))
        {
        }
    }
}
