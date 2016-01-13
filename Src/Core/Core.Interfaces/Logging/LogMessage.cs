using System;
using System.Runtime.Serialization;

namespace Core.Interfaces.Logging
{
    public class LogMessage
    {
        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }

        public LogMessageSeverity Severity { get; set; }

        public LogMessageCategory Category { get; set; }

        public string FilePath { get; set; }

        public string CallerName { get; set; }

        public int LineNumber { get; set; }

        public string MachineName { get; set; }

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public LogMessage()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
