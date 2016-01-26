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

        #region Public Methods

        public static void Main(string[] args)
        {
            if(Environment.UserInteractive)
            {
                StartService(args);
            }
            else
            {
                System.ServiceProcess.ServiceBase.Run(new HostServiceBase());
            }
        }

        public static void WindowsServiceStart()
        {
            StartService(null);
        }

        public static void WindowsServiceStop()
        {
            StopService();
        }

        #endregion

        #region Private Methods

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Log("Unhandled exception in HostService", LogMessageSeverity.Critical);
            _logger.Log(e.ExceptionObject.ToString(), LogMessageSeverity.Critical);
            StopService();
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
            _logger.Log("Ctrl+C pressed, shutting down server...");
            StopService();
        }

        private static void StartService(string[] args)
        {
            BootStrapper.Configure(IoCContainer.Instance);

            _logger = IoCContainer.Instance.Resolve<ILogger>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var logFileConfig = GetLogConfiguration();

            _logger.AddLogDestination(new FileLogDestination(logFileConfig));

            _logger.Log(string.Format("Server starting up local time - {0}", DateTime.UtcNow.ToLocalTime()));

            if (Environment.UserInteractive)
            {
                _logger.AddLogDestination(new ConsoleLogDestination());

                _logger.Log("Detected User Interactive session.");

                Console.CancelKeyPress += Console_CancelKeyPress;

                _logger.Log("Press Ctrl+C to shut down server.");
            }

            _logger.Start();

            _logger.Log("Starting up HostService");

            _hostService = new HostService();

            _logger.Log("HostService running.");

            _stopService.WaitOne();

            _logger.Log("Server shutting down...");

            _hostService.Stop();

            _logger.Log("Server shut down.");

            //last this to do is shut down logging system
            _logger.Stop();

            //last thing that happens ever
            _serviceStopped.Set();
        }

        private static void StopService()
        {
            _logger.Log("Stopping service.");
            _stopService.Set();
            _logger.Log("Waiting for service to exit.");

            if(!_serviceStopped.WaitOne(TimeSpan.FromMinutes(5)))
            {
                Environment.Exit(-1); //timeout on shutdown!
            }
        }

        #endregion
    }
}
