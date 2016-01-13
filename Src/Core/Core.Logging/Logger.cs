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
            _loggerQueue = new LogMessageQueue() { IsBlocking = true };
            InternalLogger = this;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
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
                Log(string.Format("LogDestination of type \"{0}\" added.", logDestination.GetType().Name));

                _destinations.Add(logDestination);
            }
        }

        //pass through
        public void Log(LogMessage logMessage)
        {
            if (logMessage != null)
            {
                _loggerQueue.EnqueueMessage(logMessage);
            }
        }

        public void Log(string message, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, LogMessageCategory.General, LogMessageSeverity.Information, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageSeverity severity, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, LogMessageCategory.General, severity, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageCategory category, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            Log(message, category, LogMessageSeverity.Information, callerName, callerFilePath, callerLineNumber);
        }

        public void Log(string message, LogMessageCategory category, LogMessageSeverity severity, [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var logMessage = new LogMessage();

            logMessage.Category = category;
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
                    Log(string.Format("Logger starting.", this.GetType().Name));

                    lock (_destinations)
                    {
                        foreach (var destination in _destinations)
                        {
                            destination.Start();
                        }
                    }

                    _logWorker = new Thread(new ThreadStart(LogWorker));

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
                    Log(string.Format("Logger stopping.", this.GetType().Name));

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
            IsRunning = true;

            while (IsRunning || !_loggerQueue.IsQueueEmpty)
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

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                Log("Process exiting, shutting down logging system...", LogMessageSeverity.Warning);
                Stop();
            }
        }

        #endregion
    }
}
