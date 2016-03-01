using Core.Comm.BaseClasses;
using Core.Interfaces.Components;
using Core.Interfaces.Components.IoC;
using Core.Interfaces.ServiceContracts;
using Core.IoC.Container;
using Core.Models.DataContracts;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server.Hosts
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class ComponentService : ServiceHostBase<IComponentService>, IComponentService
    {
        private IComponentServiceCallback _callback;

        public ComponentService()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IComponentServiceCallback>();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public ComponentMetadata[] GetComponents()
        {
            return IoCContainer.Instance.Resolve<IComponentManager>().GetComponents();
        }
    }
}
