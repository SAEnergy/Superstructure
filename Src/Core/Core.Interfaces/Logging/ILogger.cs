using Core.Interfaces.Base;
using System.Runtime.CompilerServices;

namespace Core.Interfaces.Logging
{
    public interface ILogger : IRunnable
    {
        void Log(LogMessage logMessage);

        void Log(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1);

        void Log(string message, LogMessageSeverity severity, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1);

        void Log(string message, LogMessageCategory category, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1);

        void Log(string message, LogMessageCategory category, LogMessageSeverity severity, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1);

        void AddLogDestination(ILogDestination logDestination);

        void RemoveLogDestination(ILogDestination logDestination);
    }
}
