using Core.Interfaces.Base;
using Core.Interfaces.Logging;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm.BaseClasses
{
    public abstract class RESTHostBase : IHost
    {
        #region Properties

        public bool IsRunning { get; private set; }

        protected ILogger Logger { get; private set; }

        public Type InterfaceType { get; private set; }

        #endregion

        #region Constructor

        protected RESTHostBase()
        {
            Logger = IoCContainer.Instance.Resolve<ILogger>();

            InterfaceType = FindInterfaceType();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            Logger.Log(string.Format("Host interface of type \"{0}\" starting...", InterfaceType.Name));
        }

        public void Stop()
        {
            Logger.Log(string.Format("Host interface of type \"{0}\" stopping...", InterfaceType.Name));
        }

        #endregion

        #region Private Methods

        private Type FindInterfaceType()
        {
            return GetType().GetInterfaces().Where(i => i != typeof(IHost) && i != typeof(IRunnable)).FirstOrDefault();
        }

        #endregion
    }
}
