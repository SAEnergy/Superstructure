using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public class Subscription<T> : ISubscription<T>
    {
        public event EventHandler Connected;
        public event SubscriptionDisconnectedEvent Disconnected;

        public T Channel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Start()
        {
        }

        protected void OnConnect()
        {
            if (Connected != null ) { Connected(this, null); }
        }

        protected void OnDisconnect(Exception ex)
        {
            if (Disconnected !=null) { Disconnected(this, ex); }
        }

        public void Stop()
        {
        }

        public bool Verify()
        {
            throw new NotImplementedException();
        }
    }
}
