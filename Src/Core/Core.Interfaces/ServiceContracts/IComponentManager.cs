using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IComponentManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void MooBack(string moo);
    }

    [ServiceContract(CallbackContract = typeof(IComponentManagerCallback))]
    public interface IComponentManager : IUserAuthentication
    {
        [OperationContract]
        void Stop();
    }
}
