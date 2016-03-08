using Core.Comm.BaseClasses;
using Core.Interfaces.Components;
using Core.Interfaces.ServiceContracts;
using Core.IoC.Container;
using Core.Models.DataContracts;
using System.ServiceModel;

namespace Server.Hosts
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class ComponentHost : ServiceHostBase<IComponentService, IComponentServiceCallback>, IComponentService
    {
        public ComponentHost()
        {
        }

        public ComponentMetadata[] GetComponents()
        {
            return IoCContainer.Instance.Resolve<IComponentManager>().GetComponents();
        }

        public void Start(int componentId)
        {
            var rc = IoCContainer.Instance.Resolve<IComponentManager>().StartComponent(componentId);
            UpdateAllUsers(rc);
        }

        public void Restart(int componentId)
        {
            var rc = IoCContainer.Instance.Resolve<IComponentManager>().RestartComponent(componentId);
            UpdateAllUsers(rc);
        }

        public void Stop(int componentId)
        {
            var rc = IoCContainer.Instance.Resolve<IComponentManager>().StopComponent(componentId);
            UpdateAllUsers(rc);
        }

        public void Disable(int componentId)
        {
            var rc = IoCContainer.Instance.Resolve<IComponentManager>().DisableComponent(componentId);
            UpdateAllUsers(rc);
        }

        #region Private Methods

        private void UpdateAllUsers(ComponentMetadata data)
        {
            if (data != null)
            {
                Broadcast((IComponentServiceCallback c) => c.ComponentUpdated(data));
            }
        }

        #endregion
    }
}
