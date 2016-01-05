using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging
{
    public class Logger : ILogger
    {
        #region Fields

        private List<ILogDestination> _destinations;
        private Thread _logWorker;
        private ManualResetEvent _logWorkerDone;
        private Queue<LogMessage> _logQueue;

        private static object _syncObject = new object();

        #endregion

        #region Properties

        public IReadOnlyList<ILogDestination> LogDestinations
        {
            get
            {
                return _destinations.AsReadOnly();
            }
        }

        public static bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        public Logger()
        {
            _destinations = new List<ILogDestination>();
            _logQueue = new Queue<LogMessage>();
        }

        #endregion

        #region Public Methods

        public void AddLogDestination(ILogDestination logDestination)
        {
            lock(_destinations)
            {
                _destinations.Add(logDestination);
            }
        }

        public void Log(LogMessage logMessage)
        {
            lock(_logQueue)
            {
                _logQueue.Enqueue(logMessage);
            }
        }

        public void RemoveLogDestination(ILogDestination logDestination)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            lock (_syncObject)
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    _logWorkerDone = new ManualResetEvent(false);

                    _logWorker = new Thread(new ThreadStart(LogWorker));

                    _logWorker.IsBackground = true;
                    _logWorker.Start();
                }
            }
        }

        public void Stop()
        {
            lock(_syncObject)
            {
                if (IsRunning)
                {
                    IsRunning = false;

                    _logWorkerDone.WaitOne();
                }
            }
        }

        #endregion

        #region Private Methods

        private void LogWorker()
        {
            while(IsRunning)
            {
                var messages = GetMessages();
            }

            _logWorkerDone.Set();
        }

        private List<LogMessage> GetMessages()
        {
            var list = new List<LogMessage>();



            return list;
        }

        #endregion
    }
}
