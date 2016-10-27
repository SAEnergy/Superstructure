using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Base
{
    public class ViewModelBase : DependencyObject, IDisposable
    {
        protected SynchronizationContext _context;
        protected ViewBase _parent;

        public ViewModelBase(ViewBase parent)
        {
            _parent = parent;
            _context = SynchronizationContext.Current;
        }

        public virtual void Dispose() { }

        protected virtual void HandleTransactionException(Exception error)
        {
            //todo: log when logger is present in client
        }

        protected void Invoke(Action task)
        {
            _context.Send(delegate { task(); }, null);
        }

        protected void BeginInvoke(Action task)
        {
            _context.Post(delegate { task(); }, null);
        }

        protected void RevalidateAllCommands()
        {
            // Go through all of the commands in this view model and trigger a re-evaluation of the CanExecute flag.
            foreach (PropertyInfo prop in this.GetType().GetProperties().Where(p => typeof(SimpleCommand).IsAssignableFrom(p.PropertyType)))
            {
                SimpleCommand commie = (SimpleCommand) prop.GetValue(this);
                commie.FireCanExecuteChangedEvent();
            }
        }
    }

    public class ViewModelBase<T> : ViewModelBase where T : IUserAuthentication
    {
        protected Subscription<T> _sub;

        public ViewModelBase(ViewBase parent) : base(parent)
        {
            _sub = new Subscription<T>(ServerConnectionInformation.Instance,this);
            _sub.Connected += OnConnect;
            _sub.Disconnected += OnDisconnect;
            _sub.Start();
        }

        protected T Channel { get { return _sub.Channel; } }

        protected virtual void OnConnect(ISubscription source)
        {
            RevalidateAllCommands();
        }

        protected virtual void OnDisconnect(ISubscription source, Exception error)
        {
            RevalidateAllCommands();
            HandleTransactionException(error);
        }

        public override void Dispose()
        {
            _sub.Stop();
            base.Dispose();
        }

        protected Task Execute(Action action)
        {
            return Task.Run(()=> 
            {
                try
                {
                    action();
                }
                catch(Exception ex)
                {
                    HandleTransactionException(ex);
                }
            });
        }
    }
}
