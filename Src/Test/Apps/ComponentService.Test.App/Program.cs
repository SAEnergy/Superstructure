using Core.Interfaces.Components.Logging;
using Core.Logging;
using Core.Logging.LogDestinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComponentService.Test.App
{
    public class Program
    {
        private static ILogger _logger;
        private static ManualResetEvent _stop = new ManualResetEvent(false);
        private static ManualResetEvent _exit = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            _logger = Logger.CreateInstance();

            _logger.AddLogDestination(new ConsoleLogDestination());

            _logger.Start();

            Console.CancelKeyPress += Console_CancelKeyPress;

            ComponentServiceAbuser abuser = new ComponentServiceAbuser(_logger);

            abuser.Start();

            _stop.WaitOne();

            abuser.Stop();

            _logger.Stop();

            _exit.Set();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _stop.Set();
            _exit.WaitOne();
        }
    }
}
