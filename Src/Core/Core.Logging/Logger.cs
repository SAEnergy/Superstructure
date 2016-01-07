using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Core.Logging
{
    public class Logger : ILogger
    {
        #region Fields

        private List<ILogDestination> _destinations;
        private Thread _logWorker;
        private LogMessageQueue _loggerQueue;

        private static object _syncObject = new object();

        #endregion

        #region Properties

        public IReadOnlyList<ILogDestination> LogDestinations
        {
            get
            {
                lock (_destinations)
                {
                    return _destinations.AsReadOnly();
                }
            }
        }

        internal static ILogger InternalLogger { get; private set; }

        public bool IsRunning { get; private set; }

        #endregion

        #region Constructor

        public Logger()
        {
            _destinations = new List<ILogDestination>();
            _loggerQueue = new LogMessageQueue();
            InternalLogger = this;
        }

        #endregion

        #region Public Methods

        public void AddLogDestination(ILogDestination logDestination)
        {
            if(IsRunning)
            {
                logDestination.Start();
            }

            lock(_destinations)
            {
                Log(LogMessageSeverity.Information, string.Format("LogDestination of type \"{0}\" added.", logDestination.GetType().Name));

                _destinations.Add(logDestination);
            }
        }

        public void Log(LogMessage logMessage, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (logMessage != null)
            {
                logMessage.CallerName = callerName;
                logMessage.FilePath = callerFilePath;
                logMessage.LineNumber = callerLineNumber;

                _loggerQueue.EnqueueMessage(logMessage);
            }
        }

        public void Log(LogMessageSeverity severity, string message, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var logMessage = new LogMessage();

            logMessage.Severity = severity;
            logMessage.Message = message;
            logMessage.CallerName = callerName;
            logMessage.FilePath = callerFilePath;
            logMessage.LineNumber = callerLineNumber;

            _loggerQueue.EnqueueMessage(logMessage);
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
                    Log(LogMessageSeverity.Information, string.Format("Logger starting.", this.GetType().Name));

                    IsRunning = true;

                    lock (_destinations)
                    {
                        foreach (var destination in _destinations)
                        {
                            destination.Start();
                        }
                    }

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
                    Log(LogMessageSeverity.Information, string.Format("Logger stopping.", this.GetType().Name));

                    IsRunning = false;

                    _logWorker.Join();

                    lock (_destinations)
                    {
                        foreach (var destination in _destinations)
                        {
                            destination.Stop();
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void LogWorker()
        {
            while(IsRunning || !_loggerQueue.IsQueueEmpty)
            {
                //will block for max of the timespan timeout, or return a list the size of the batch size constant
                var messages = _loggerQueue.DequeueMessages();

                if (messages.Count > 0)
                {
                    lock (_destinations)
                    {
                        foreach (var destination in _destinations)
                        {
                            destination.ProcessMessages(messages);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
