using Core.Interfaces.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;

namespace Server.Hosts
{
    public class RemoteLoggingHost : ServiceHostBase<IRemoteLogging>, IRemoteLogging
    {
        public void Log(LogMessage[] messages)
        {
            foreach (LogMessage message in messages) { _logger.Log(message); }
        }
    }
}
