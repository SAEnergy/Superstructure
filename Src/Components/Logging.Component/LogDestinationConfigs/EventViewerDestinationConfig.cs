using Core.Interfaces.Components.Logging;

namespace Core.Logging.LogDestinationConfigs
{
    public class EventViewerDestinationConfig
    {
        public ILogMessageFormatter LogMessageFormatter { get; set; }

        public string Source { get; set;  }

        public string LogName { get; set; }
    }
}
