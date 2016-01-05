using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public class ConsoleLogDestination : ILogDestination
    {
        public void ProcessMessages(List<LogMessage> messages)
        {
            throw new NotImplementedException();
        }
    }
}
