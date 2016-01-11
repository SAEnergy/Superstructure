using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Core.Logging
{
    public class LogMessageQueue
    {
        #region Fields

        private TimeSpan _queueTimeOut = new TimeSpan(0, 0, 0, 0, 100); // milliseconds
        private const int _queueBatchSize = 50;
        private const int _defaultMaxQueueSize = 5000;

        private Queue<LogMessage> _logQueue;

        #endregion

        #region Properties

        public bool IsQueueEmpty
        {
            get
            {
                return _logQueue.Count == 0;
            }
        }

        public bool IsBlocking { get; set; }

        public int MaxQueueSize { get; set; }

        #endregion

        #region Constructor

        public LogMessageQueue()
        {
            _logQueue = new Queue<LogMessage>();
            MaxQueueSize = _defaultMaxQueueSize;
        }

        #endregion

        #region Public Methods

        public void EnqueueMessages(List<LogMessage> messages)
        {
            CheckQueue();

            lock(_logQueue)
            {
                foreach(var message in messages)
                {
                    _logQueue.Enqueue(message);
                }
            }
        }

        public void EnqueueMessage(LogMessage message)
        {
            CheckQueue();

            lock (_logQueue)
            {
                _logQueue.Enqueue(message);
            }
        }

        public List<LogMessage> DequeueMessages()
        {
            var list = new List<LogMessage>();
            var sleepTime = Math.Max(_queueTimeOut.Milliseconds / _queueBatchSize, 1); //regardless of what a silly user might set, don't let the sleep get below 1 millisecond
            var timeOutTime = DateTime.UtcNow.Add(_queueTimeOut);

            while (list.Count < _queueBatchSize && DateTime.UtcNow < timeOutTime)
            {
                if (_logQueue.Count > 0)
                {
                    //lock here so you can enqueue while we are still dequeuing in worker threads
                    lock (_logQueue)
                    {
                        //while we have a lock on the queue, burn threw it till we empty it or fill ourselves up
                        while (list.Count < _queueBatchSize && _logQueue.Count > 0)
                        {
                            list.Add(_logQueue.Dequeue());
                        }
                    }
                }

                Thread.Sleep(sleepTime);
            }

            return list;
        }

        #endregion

        #region Private Methods

        private void CheckQueue()
        {
            if(_logQueue.Count > MaxQueueSize)
            {
                if(IsBlocking)
                {
                    while(_logQueue.Count > MaxQueueSize)
                    {
                        Thread.Sleep(_queueTimeOut);
                    }
                }
                else
                {
                    lock(_logQueue)
                    {
                        _logQueue.Clear();
                        _logQueue.Enqueue(CreateLogMessage(LogMessageSeverity.Warning, string.Format("This LogDestination's internal queue size of {0} has been overwhelmed.  This has not been configured to block, so messages have been lost.", MaxQueueSize)));
                    }
                }
            }
        }

        private LogMessage CreateLogMessage(LogMessageSeverity severity, string message, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            var logMessage = new LogMessage();

            logMessage.CallerName = callerName;
            logMessage.FilePath = callerFilePath;
            logMessage.Message = message;
            logMessage.Severity = severity;
            logMessage.LineNumber = callerLineNumber;

            return logMessage;
        }

        #endregion
    }
}
