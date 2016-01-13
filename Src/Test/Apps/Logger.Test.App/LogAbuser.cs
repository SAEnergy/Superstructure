using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerTest.App
{
    public class LogAbuser
    {
        private Thread abuseThread;
        private readonly ILogger _logger;
        private static bool _isRunning;
        private ManualResetEvent _stopped;

        public int SleepTimeInMilliseconds { get; set; }

        public LogAbuser(ILogger logger)
        {
            _logger = logger;

            Start();
        }

        public void Start()
        {
            _stopped = new ManualResetEvent(false);

            abuseThread = new Thread(new ThreadStart(AbusiveWorker));
            abuseThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;

            _stopped.WaitOne();
        }

        private void AbusiveWorker()
        {
            _isRunning = true;

            while(_isRunning)
            {
                _logger.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore");
                _logger.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore");
                _logger.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore");
                _logger.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore");

                _logger.Log("Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat", LogMessageSeverity.Warning);
                _logger.Log("Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat", LogMessageSeverity.Warning);

                _logger.Log("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur", LogMessageSeverity.Error);
                _logger.Log("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur", LogMessageSeverity.Error);
                _logger.Log("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur", LogMessageSeverity.Error);

                _logger.Log("Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", LogMessageSeverity.Critical);

                Thread.Sleep(SleepTimeInMilliseconds);
            }

            _stopped.Set();
        }

    }
}
