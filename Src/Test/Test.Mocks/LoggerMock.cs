using Core.Interfaces.Logging;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Test.Mocks
{
    public class LoggerMock : ILogger
    {
        public bool IsRunning { get; private set; }

        public void AddLogDestination(ILogDestination logDestination)
        {
            //ignore
        }

        public void Log(LogMessage logMessage, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Trace.TraceInformation(logMessage.Message);
        }

        public void Log(LogMessageSeverity severity, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
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
    }
}
