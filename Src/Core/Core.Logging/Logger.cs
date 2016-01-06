using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public void Log(LogMessage logMessage, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            logMessage.CallerName = callerName;
            logMessage.FilePath = callerFilePath;
            logMessage.LineNumber = callerLineNumber;

            EnqueLogMessage(logMessage);
        }

        public void Log(LogMessageSeverity severity, string message, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1, params object[] args)
        {
            var logMessage = new LogMessage();

            logMessage.Severity = severity;
            logMessage.Message = message;
            logMessage.CallerName = callerName;
            logMessage.FilePath = callerFilePath;
            logMessage.LineNumber = callerLineNumber;

            EnqueLogMessage(logMessage);
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

        private void EnqueLogMessage(LogMessage logMessage)
        {
            lock (_logQueue)
            {
                _logQueue.Enqueue(logMessage);
            }
        }

        private void LogWorker()
        {
            while(IsRunning)
            {
                //will block for a moment and return a list of messages which will be handed off to all destinations
                var messages = GetMessages();

                lock(_destinations)
                {
                    foreach(var destination in _destinations)
                    {
                        destination.ProcessMessages(messages);
                    }
                }
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
