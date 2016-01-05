using Core.Interfaces.Logging;
using Core.IoC.Container;
using Core.Logging.LogDestinations;
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

            if (Environment.UserInteractive)
            {
                _logger.AddLogDestination(new ConsoleLogDestination());

                Console.CancelKeyPress += Console_CancelKeyPress;
            }

            _logger.Start();

            _hostService = new HostService();

            _stopService.WaitOne();

            _logger.Stop();

            //last thing that happens ever
            _serviceStopped.Set();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _stopService.Set();
            _serviceStopped.WaitOne();
        }

        #endregion
    }
}
