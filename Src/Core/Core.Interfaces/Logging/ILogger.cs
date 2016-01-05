using Core.Interfaces.Base;

namespace Core.Interfaces.Logging
{
    public interface ILogger : IRunnable
    {
        void Log(LogMessage logMessage);

        void AddLogDestination(ILogDestination logDestination);

        void RemoveLogDestination(ILogDestination logDestination);
    }
}
