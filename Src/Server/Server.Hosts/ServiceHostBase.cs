using Core.Comm;
using Core.Interfaces.Base;
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
    public class ServiceHostBase<T> : IServiceHost where T : class // todo: base class for service contracts
    {
        public Type InterfaceType
        {
            get { return typeof(T); }
        }

        public static Type GetInterfaceType()
        {
            return typeof(T);
        }
    }
}
