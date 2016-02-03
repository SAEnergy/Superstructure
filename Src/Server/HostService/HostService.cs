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

                StartAllRunnableTypes();

                IoCContainer.Instance.Resolve<IHostManagerComponent>().StartAll();
            }
        }

        public void Stop()
        {
            if(IsRunning)
            {
                IsRunning = false;
                _heartBeat.Stop();

                StopAllRunnableTypes();

                IoCContainer.Instance.Resolve<IHostManagerComponent>().StopAll();
            }
        }

        #endregion

        #region Private Methods

        private void StartAllRunnableTypes()
        {
            _logger.Log("Starting all runnable components");

            foreach (var type in GetRunnableRegisteredTypes())
            {
                _logger.Log(string.Format("Starting component of type {0}", type.Name));

                var runnable = IoCContainer.Instance.Resolve(type) as IRunnable;

                if(runnable != null)
                {
                    runnable.Start();
                }
                else
                {
                    _logger.Log(string.Format("Failed to get runnable object from type {0}.", type.Name), LogMessageSeverity.Error);
                }
            }
        }

        private void StopAllRunnableTypes()
        {
            _logger.Log("Stopping all runnable components");

            foreach (var type in GetRunnableRegisteredTypes())
            {
                _logger.Log(string.Format("Stopping component of type {0}", type.Name));

                var runnable = IoCContainer.Instance.Resolve(type) as IRunnable;

                if (runnable != null)
                {
                    runnable.Stop();
                }
                else
                {
                    _logger.Log(string.Format("Failed to get runnable object from type {0}.", type.Name), LogMessageSeverity.Error);
                }
            }
        }

        private List<Type> GetRunnableRegisteredTypes()
        {
            return IoCContainer.Instance.GetRegisteredTypes().Where(i => typeof(IRunnable).IsAssignableFrom(i) && i != typeof(ILogger)).ToList();
        }

        #endregion
    }
}
