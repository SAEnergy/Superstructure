using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.Interfaces.Services;
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
        #region Fields

        private Heartbeat _heartBeat;

        private readonly ILogger _logger;

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        public HostService()
        {
            _logger = IoCContainer.Instance.Resolve<ILogger>();

            Start();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            if(!IsRunning)
            {
                IsRunning = true;
                _heartBeat = new Heartbeat();

                IoCContainer.Instance.Resolve<IHostManagerService>().StartAll();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                IsRunning = false;
                _heartBeat.Stop();

                IoCContainer.Instance.Resolve<IHostManagerService>().StopAll();
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
