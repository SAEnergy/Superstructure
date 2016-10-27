using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IUserAuthentication
    {
        [OperationContract]
        void Ping();
    }
}
