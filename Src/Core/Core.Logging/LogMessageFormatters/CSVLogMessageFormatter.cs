using Core.Interfaces.Logging;

namespace Core.Logging.LogMessageFormats
{
    public class CSVLogMessageFormatter : ILogMessageFormatter
    {
        public string Format(LogMessage message)
        {
            return string.Format("{0},{1},\"{2}\",{3},{4},{5},{6},{7}", message.TimeStamp,
                message.Severity, message.Message, message.ProcessName, message.ProcessId,
                message.FilePath, message.CallerName, message.LineNumber);
        }

        public string GetHeader()
        {
            return "TimeStamp,Severity,Message,ProcessName,ProcessId,FilePath,CallerName,LineNumber";
        }
    }
}
