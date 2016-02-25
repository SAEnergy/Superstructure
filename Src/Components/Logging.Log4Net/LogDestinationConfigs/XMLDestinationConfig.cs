using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.Logging;

namespace Core.Logging.Log4Net.LogDestinationConfigs
{
    public class XMLDestinationConfig
    {
        public ILogMessageFormatter LogMessageFormatter { get; set; }

        public int MaxLogFileSize { get; set; } //megabytes

        public string LogDirectory { get; set; }

        public string LogFilePrefix { get; set; }

    }

}
