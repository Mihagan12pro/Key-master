using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Key_master.WPF
{
    internal class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private Action<object> execute;//Сама функция
        private Func<object, bool> canExucute;//Нужно, чтобы контролировать, когда кнопка активна, а когда нет

        public event EventHandler CanExucuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public Command(Action<object> execute, Func<object, bool> canExucute = null)
        {
            this.execute = execute;
            this.canExucute = canExucute;
        }

        public bool CanExecute(object parameter)
        {
            return canExucute == null || canExucute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
