using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Logging
{
    public interface ILogDestination
    {
        void ProcessMessages(List<LogMessage> messages);
    }
}
