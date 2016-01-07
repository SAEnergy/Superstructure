using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HostService
{
    public class HostService : IRunnable
    {
        private Heartbeat _heartBeat;

        public bool IsRunning { get; private set; }

        public HostService()
        {
            Start();
        }

        public void Start()
        {
            if(!IsRunning)
            {
                IsRunning = true;
                _heartBeat = new Heartbeat();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                IsRunning = false;
                _heartBeat.Stop();
            }
        }
    }
}
