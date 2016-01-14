using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.ComponentModel;
using System.Threading;


namespace HostService
{
    public class HostServiceBase : ServiceBase
    {
        IContainer _components;
        private Thread _hostThread;
        private TimeSpan _shutdownWaitTime; 


        public HostServiceBase()
        {
            _components = new Container();
            _hostThread = null;
            _shutdownWaitTime = TimeSpan.FromSeconds(120); // just under Windows default which is 125

            ServiceName = "Host Service";

            CanPauseAndContinue = false;
            CanShutdown = true;
            CanStop = true;
        }


        protected override void OnStart(string[] args)
        {
            // TODO: This triggers a debugger to fix problems with the service.  Can this be a configuration flag somehow?
            //System.Diagnostics.Debugger.Launch();

            _hostThread = new Thread(new ThreadStart(Program.WindowsServiceStart));
            _hostThread.Start();
        }

        protected override void OnStop()
        {
            if(_hostThread.IsAlive)
            {
                Program.WindowsServiceStop();
                _hostThread.Join(_shutdownWaitTime);
            }
        }

        protected override void OnShutdown()
        {
            OnStop();
        }
    }
}
