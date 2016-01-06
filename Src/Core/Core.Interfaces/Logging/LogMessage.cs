using System;
using System.Runtime.Serialization;

namespace Core.Interfaces.Logging
{
    [DataContract]
    public class LogMessage
    {
        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public LogMessageSeverity Severity { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public string CallerName { get; set; }

        [DataMember]
        public int LineNumber { get; set; }

        [DataMember]
        public int ProcessId { get; set; }

        [DataMember]
        public string ProcessName { get; set; }

        public LogMessage()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
