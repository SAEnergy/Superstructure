using Core.Interfaces.Logging;

namespace Core.Logging.LogDestinationConfigs
{
    public class TCPLogDestinationConfig
    {
        public ILogMessageFormatter LogMessageFormatter { get; set; }

        public string HostName { get; set;  }

        public int Port { get; set; }
    }
}
