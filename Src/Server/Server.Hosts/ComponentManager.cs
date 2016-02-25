using Core.Comm.BaseClasses;
using Core.Interfaces.ServiceContracts;
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
    }
}
