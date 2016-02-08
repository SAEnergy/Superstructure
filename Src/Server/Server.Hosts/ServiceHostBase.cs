using Core.Comm;
using Core.Interfaces.Base;
using Core.Interfaces.Components.Logging;
using Core.Interfaces.ServiceContracts;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Server.Hosts
{
    public class ServiceHostBase<T> : IServiceHost, IUserAuthentication, IDisposable  where T : IUserAuthentication
    {
        protected ILogger _logger;

        public Type InterfaceType
        {
            get { return typeof(T); }
        }

        // called via reflection to find implemented service contract
        public static Type GetInterfaceType()
        {
            return typeof(T);
        }


        public void Ping()
        {
            // todo: real authentication
        }

        public virtual void Dispose()
        {
            _logger.Log("Host of type " + this.GetType() + " closed.");
        }

        public ServiceHostBase()
        {
            _logger = (ILogger) IoCContainer.Instance.Resolve(typeof(ILogger));
            _logger.Log("Host of type " + this.GetType() + " created.");
        }
    }
}
