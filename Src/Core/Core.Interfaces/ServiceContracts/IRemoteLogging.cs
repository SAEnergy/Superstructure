using Core.Interfaces.Components.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.ServiceContracts
{
    [ServiceContract]
    public interface IRemoteLogging : IUserAuthentication
    {
        [OperationContract]
        void Log(LogMessage[] messages);
    }
}
