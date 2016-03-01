using Core.Models.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IComponentServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void MooBack(string moo);
    }

    [ServiceContract(CallbackContract = typeof(IComponentServiceCallback))]
    public interface IComponentService : IUserAuthentication
    {
        [OperationContract]
        void Stop(int componentId);

        [OperationContract]
        void Start(int componentId);

        [OperationContract]
        void Restart(int componentId);

        [OperationContract]
        ComponentMetadata[] GetComponents();
    }
}
