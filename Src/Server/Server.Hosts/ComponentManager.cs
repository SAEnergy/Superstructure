using Core.Comm.BaseClasses;
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
    public class ComponentManager : ServiceHostBase<IComponentManager>, IComponentManager
    {
        private IComponentManagerCallback _callback;

        public ComponentManager()
        {
            _callback = OperationContext.Current.GetCallbackChannel<IComponentManagerCallback>();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public List<ComponentInfo> GetComponents()
        {
            var infos = new List<ComponentInfo>();

            foreach(var type in IoCContainer.Instance.GetRegisteredTypes())
            {
                var info = new ComponentInfo();
                info.TypeName = type.FullName;

                //var atty = type.GetAttribute<ComponentMetadataAttribute>();

                //if(atty != null)
                //{
                //    info.Description = atty.Description;
                //    info.FriendlyName = atty.FriendlyName;
                //}

                infos.Add(info);
            }

            return infos;
        }
    }
}
