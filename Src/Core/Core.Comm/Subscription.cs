using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Core.Comm
{
    public class Subscription<T> : ISubscription<T>
    {
        public event EventHandler Connected;
        public event SubscriptionDisconnectedEvent Disconnected;
        protected DuplexChannelFactory<T> _factory;
        protected object _callback;
        protected Exception _lastException;
        private Thread _worker;

        public Subscription(object callback)
        {
            _callback = callback;
        }
        public T Channel { get; protected set; }

        public void Start()
        {
            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9595/tcp/" + typeof(T).Name + "/");
            Binding binding = new NetTcpBinding(SecurityMode.None, false);

            _factory = new DuplexChannelFactory<T>(_callback, binding, endpoint);
            _factory.Opened += _factory_Opened;
            _factory.Faulted += _factory_Faulted;
            _worker = new Thread(new ThreadStart(WorkerThread));
            _worker.Start();
        }

        private void WorkerThread()
        {
            while (true)
            {
                if (_factory.State != CommunicationState.Opened) { }
                try
                {
                    _factory.Open();
                }
                catch (ThreadAbortException)
                {
                    try
                    {
                        if (_factory != null) { _factory.Abort(); }
                    }
                    catch
                    {
                        /* nom */
                    }
                }
                catch (Exception ex)
                {
                    _lastException = ex;
                }
            }
        }

        private void _factory_Opened(object sender, EventArgs e)
        {
            Channel = _factory.CreateChannel();
            if (Connected != null) { Connected(this, null); }
        }

        private void _factory_Faulted(object sender, EventArgs e)
        {
            if (Disconnected != null) { Disconnected(this, _lastException); }
        }

        protected void OnConnect()
        {
        }

        protected void OnDisconnect(Exception ex)
        {
        }

        public void Stop()
        {
            _worker.Abort();
        }

        public bool Verify()
        {
            throw new NotImplementedException();
        }
    }
}
