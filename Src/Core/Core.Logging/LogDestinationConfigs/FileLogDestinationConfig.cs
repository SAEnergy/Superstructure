using Core.Interfaces.Logging;

namespace Core.Logging.LogDestinationConfigs
{
    public class FileLogDestinationConfig
    {
        public ILogMessageFormatter LogMessageFormatter { get; set; }

        public int MaxLogFileCount { get; set; }

        public int MaxLogFileSize { get; set; } //megabytes

        public string LogDirectory { get; set; }

        public string LogFilePrefix { get; set; }

        public string LogFileExtension { get; set; }
    }
}
