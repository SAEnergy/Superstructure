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
    public class ServiceHostBase<T> : IHost where T : class // todo: base class for service contracts
    {
        public Type InterfaceType
        {
            get { return typeof(T); }
        }

        public bool IsRunning { get; protected set; }

        public void Start()
        {

            ServiceHost host = new ServiceHost(this.GetType());

            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:9595/tcp/" + InterfaceType.Name + "/");

            Binding binding = new NetTcpBinding(SecurityMode.None, false);

            ServiceEndpoint service = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, endpoint);

            host.AddServiceEndpoint(service);

            host.Open();

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
