using Core.Comm.BaseClasses;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;

namespace Server.Hosts
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class DuplexTestHost : ServiceHostBase<IDuplexTest>, IDuplexTest, IDisposable
    {
        private IDuplexTestCallback _callback;
        private Thread _worker;

        public DuplexTestHost()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IDuplexTestCallback>();
            _worker = new Thread(new ThreadStart(WorkerThread));
            _worker.Start();
        }

        private void WorkerThread()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    _callback.MooBack("At the pong, the time will be "+DateTime.Now.ToLongTimeString());
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
            _worker.Abort();
        }

        public bool Moo()
        {
            return true;
        }
    }
}
