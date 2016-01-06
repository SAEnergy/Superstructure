using Core.Interfaces.Base;
using System.Collections.Generic;

namespace Core.Interfaces.Logging
{
    public interface ILogDestination : IRunnable
    {
        void ProcessMessages(List<LogMessage> messages);
    }
}
