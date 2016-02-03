using Core.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Interfaces.Components.Logging
{
    public interface ILogDestination : IRunnable
    {
        void Flush();

        void ProcessMessages(List<LogMessage> messages);

        void HandleLoggingException(LogMessage message);
    }
}
