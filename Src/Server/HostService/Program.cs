using Core.Interfaces.Logging;
using Core.IoC.Container;
using Core.Logging.LogDestinationConfigs;
using Core.Logging.LogDestinations;
using Core.Logging.LogMessageFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HostService
{
    public class Program
    {
        #region Fields

        private static ILogger _logger;
        private static HostService _hostService;
        private static ManualResetEvent _stopService = new ManualResetEvent(false);
        private static ManualResetEvent _serviceStopped = new ManualResetEvent(false);

        #endregion

        #region Main

        public static void Main(string[] args)
        {
            BootStrapper.Configure(IoCContainer.Instance);

            _logger = IoCContainer.Instance.Resolve<ILogger>();

            var logFileConfig = GetLogConfiguration();

            _logger.AddLogDestination(new FileLogDestination(logFileConfig));

            _logger.Log(LogMessageSeverity.Information, string.Format("Server starting up local time - {0}", DateTime.Now));

            if (Environment.UserInteractive)
            {
                _logger.AddLogDestination(new ConsoleLogDestination());

                _logger.Log(LogMessageSeverity.Information, "Detected User Interactive session.");

                Console.CancelKeyPress += Console_CancelKeyPress;

                _logger.Log(LogMessageSeverity.Information, "Press Ctrl+C to shut down server.");
            }

            _logger.Start();

            _logger.Log(LogMessageSeverity.Information, "Starting up HostService");

            _hostService = new HostService();

            _logger.Log(LogMessageSeverity.Information, "HostService running.");

            _stopService.WaitOne();

            _logger.Log(LogMessageSeverity.Information, "Server shutting down...");

            _hostService.Stop();

            _logger.Log(LogMessageSeverity.Information, "Server shut down.");

            //last this to do is shut down logging system
            _logger.Stop();

            //last thing that happens ever
            _serviceStopped.Set();
        }

        private static FileLogDestinationConfig GetLogConfiguration()
        {
            var logFileConfig = new FileLogDestinationConfig();

            //hardcode for now
            logFileConfig.LogDirectory = Configuration.Instance.LogDirectory;
            logFileConfig.LogFileExtension = Configuration.Instance.LogFileExtension;
            logFileConfig.LogFilePrefix = Configuration.Instance.LogFilePrefix;
            logFileConfig.LogMessageFormatter = BuildFormatter(Configuration.Instance.LogMessageFormatter);
            logFileConfig.MaxLogFileSize = Configuration.Instance.MaxLogFileSize;
            logFileConfig.MaxLogFileCount = Configuration.Instance.MaxLogFileCount;

            return logFileConfig;
        }

        private static ILogMessageFormatter BuildFormatter(string typeName)
        {
            //hard code this for now
            return new CSVLogMessageFormatter();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _logger.Log(LogMessageSeverity.Information, "Ctrl+C pressed, shutting down server...");
            _stopService.Set();
            _serviceStopped.WaitOne();
        }

        #endregion
    }
}
