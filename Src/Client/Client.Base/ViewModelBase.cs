using Core.Comm;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Base
{
    public class ViewModelBase : IDisposable
    {
        protected SynchronizationContext _context;
        protected ViewBase _parent;
        
        public ViewModelBase(ViewBase parent)
        {
            _parent = parent;
            _context = SynchronizationContext.Current;
        }

        public virtual void Dispose() {  }

        protected virtual void HandleTransactionException(Exception error)
        {
            //todo: log when logger is present in client
        }

        protected void InvokeIfRequired(Action task)
        {
            _context.Send(delegate { task(); },null);
        }

        protected void BeginInvokeIfRequired(Action task)
        {
            _context.Post(delegate { task(); }, null);
        }
    }

    public class ViewModelBase<T> : ViewModelBase where T : IUserAuthentication
    {
        protected Subscription<T> _conn;

        public ViewModelBase(ViewBase parent) : base(parent)
        {
            _conn = new Subscription<T>(this);
            _conn.Connected += OnConnect;
            _conn.Disconnected += OnDisconnect;
            _conn.Start();
        }

        protected T Channel { get; }

        protected virtual void OnConnect(ISubscription source)
        {

        }

        protected virtual void OnDisconnect(ISubscription source, Exception error)
        {
            HandleTransactionException(error);
        }

        public override void Dispose()
        {
            _conn.Stop();
            base.Dispose();
        }
    }
}
