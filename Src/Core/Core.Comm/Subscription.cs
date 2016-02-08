using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Core.Interfaces.ServiceContracts;

namespace Core.Comm
{

    public enum SubscriptionState { Disconnected, Connecting, Connected, };

    public class Subscription<T> : ISubscription<T> where T : IUserAuthentication
    {
        public event EventHandler Connected;
        public event SubscriptionDisconnectedEvent Disconnected;
        protected DuplexChannelFactory<T> _factory;
        protected object _callback;
        protected Exception _lastException;
        private Thread _worker;
        public SubscriptionState State { get; private set; }

        private object _stateLock = new object();

        public Subscription(object callback)
        {
            _callback = callback;
        }
        public T Channel { get; protected set; }

        public void Start()
        {
            lock (_stateLock)
            {
                State = SubscriptionState.Connecting;
            }
            _worker = new Thread(new ThreadStart(WorkerThread));
            _worker.Start();
        }

        private void WorkerThread()
        {
            try
            {
                while (true)
                {
                    if (_factory == null)
                    {
                        EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9595/tcp/" + typeof(T).Name + "/");
                        Binding binding = new NetTcpBinding(SecurityMode.None, false);
                        _factory = new DuplexChannelFactory<T>(_callback, binding, endpoint);
                    }

                    if (_factory.State != CommunicationState.Opened)
                    {

                        try
                        {
                            _factory.Open();
                            Channel = _factory.CreateChannel();
                            ((ICommunicationObject)Channel).Closed += Channel_Closed;
                            ((ICommunicationObject)Channel).Faulted += Channel_Faulted;
                            Channel.Ping();
                            lock (_stateLock)
                            {
                                State = SubscriptionState.Connected;
                            }
                            if (Connected != null) { Connected(this, null); }
                        }
                        catch (ThreadAbortException)
                        {

                        }
                        catch (Exception ex)
                        {
                            _lastException = ex;
                            OnDisconnect(ex);
                        }
                    }

                    Thread.Sleep(15000);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        private void Cleanup()
        {

            Channel = default(T);

            try
            {
                if (_factory != null) { _factory.Close(); }
            }
            catch
            {
                /* nom */
            }

            try
            {
                if (_factory != null) { _factory.Abort(); }
            }
            catch
            {
                /* nom */
            }

            _factory = null;
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            OnDisconnect(new Exception("Channel Closed."));
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            OnDisconnect(new Exception("Channel Closed."));
        }

        protected void OnDisconnect(Exception ex)
        {
            try
            {
                lock (_stateLock)
                {
                    if (State == SubscriptionState.Disconnected) { return; }
                    State = SubscriptionState.Disconnected;
                }

                if (Disconnected != null) { Disconnected(this, ((ex != null) ? ex : new Exception("Unknown error."))); }
            }
            finally
            {
                Cleanup();
            }
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
