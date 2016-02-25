using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Base
{
    public delegate bool CanExecuteCallback();

    public class SimpleCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _action;
        private Action<object> _paramAction;
        private CanExecuteCallback _canExecuteCallback;
        private SynchronizationContext _context;

        public void FireCanExecuteChangedEvent()
        {
            _context.Send(delegate { if (CanExecuteChanged != null) { CanExecuteChanged(this, null); } }, null);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteCallback != null) { return _canExecuteCallback(); }
            return true;
        }

        public void Execute(object parameter)
        {
            if (_action != null) { _action(); }
            if (_paramAction != null) { _paramAction(parameter); }
        }

        public SimpleCommand()
        {
            _context = SynchronizationContext.Current;
        }

        public SimpleCommand(Action action) : this()
        {
            _action = action;
        }

        public SimpleCommand(Action action, CanExecuteCallback canExecuteCallback) : this(action)
        {
            _canExecuteCallback = canExecuteCallback;
        }

        public SimpleCommand(Action<object> parameterizedAction) : this()
        {
            _paramAction = parameterizedAction;
        }

        public SimpleCommand(Action<object> parameterizedAction, CanExecuteCallback canExecuteCallback) : this(parameterizedAction)
        {
            _canExecuteCallback = canExecuteCallback;
        }
    }
}