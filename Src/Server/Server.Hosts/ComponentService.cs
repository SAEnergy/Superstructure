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

        public ComponentMetadata[] GetComponents()
        {
            return IoCContainer.Instance.Resolve<IComponentManager>().GetComponents();
        }

        public void Start(int componentId)
        {
            IoCContainer.Instance.Resolve<IComponentManager>().StartComponent(componentId);
        }

        public void Restart(int componentId)
        {
            IoCContainer.Instance.Resolve<IComponentManager>().RestartComponent(componentId);
        }

        public void Stop(int componentId)
        {
            IoCContainer.Instance.Resolve<IComponentManager>().StopComponent(componentId);
        }

        public void Disable(int componentId)
        {
            IoCContainer.Instance.Resolve<IComponentManager>().DisableComponent(componentId);
        }
    }
}
