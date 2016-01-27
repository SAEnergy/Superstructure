using Core.Interfaces.Logging;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System;

namespace Test.Mocks
{
    public class LoggerMock : ILogger
    {
        public bool IsRunning { get; private set; }

        public void AddLogDestination(ILogDestination logDestination)
        {
            //ignore
        }

        public void HandleLoggingException(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Trace.TraceInformation(message);
        }

        public void Log(LogMessage logMessage)
        {
            Trace.TraceInformation(logMessage.Message);
        }

        public void Log(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, LogMessageCategory.General, LogMessageSeverity.Information, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageCategory category, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, category, LogMessageSeverity.Information, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageSeverity severity, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, LogMessageCategory.General, severity, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageCategory category, LogMessageSeverity severity, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Trace.TraceInformation(message);
        }

        public void RemoveLogDestination(ILogDestination logDestination)
        {
            //ignore
        }

        public void Start()
        {
            //ignore
        }

        public void Stop()
        {
            //ignore
        }

        public void Flush()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }
    }
}
