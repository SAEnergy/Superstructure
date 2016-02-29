using Core.Interfaces.Base;
using Core.Interfaces.Components;
using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;

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

                IoCContainer.Instance.Resolve<IComponentManager>().StartAll();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                IsRunning = false;
                _heartBeat.Stop();

                IoCContainer.Instance.Resolve<IComponentManager>().StopAll();
            }
        }

        #endregion
    }
}
