using Core.Interfaces.Base;
using System.Runtime.CompilerServices;

namespace Core.Interfaces.Logging
{
    public interface ILogger : IRunnable
    {
        void Log(LogMessage logMessage, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1);

        void Log(LogMessageSeverity severity, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1, params object[] args);

        void AddLogDestination(ILogDestination logDestination);

        void RemoveLogDestination(ILogDestination logDestination);
    }
}
