using Core.Interfaces.Components.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Core.Logging
{
    public class LogMessageQueue
    {
        #region Fields

        private TimeSpan _queueTimeOut = TimeSpan.FromMilliseconds(100);
        private const int _defaultQueueBatchSize = 50;
        private const int _defaultMaxQueueSize = 5000;

        private Queue<LogMessage> _logQueue;

        #endregion

        #region Properties

        public event EventHandler MessagesDropped;
        public event EventHandler MessagesBlocked;

        public bool IsQueueEmpty
        {
            get
            {
                lock (_logQueue)
                {
                    return _logQueue.Count == 0;
                }
            }
        }

        public bool IsBlocking { get; set; }

        public int MaxQueueSize { get; set; }
        public int BatchSize { get; set; }

        #endregion

        #region Constructor

        public LogMessageQueue()
        {
            _logQueue = new Queue<LogMessage>();
            MaxQueueSize = _defaultMaxQueueSize;
            BatchSize = _defaultQueueBatchSize;
        }

        #endregion

        #region Public Methods

        public void EnqueueMessages(List<LogMessage> messages)
        {
            CheckQueue();

            lock (_logQueue)
            {
                foreach (var message in messages)
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

        public void HandleLoggingException(LogMessage message)
        {
            lock (_logQueue)
            {
                _logQueue.Enqueue(message);
            }
        }


        public List<LogMessage> DequeueMessages()
        {
            var list = new List<LogMessage>();
            var sleepTime = Math.Max(_queueTimeOut.Milliseconds / BatchSize, 1); //regardless of what a silly user might set, don't let the sleep get below 1 millisecond
            var timeOutTime = DateTime.UtcNow.Add(_queueTimeOut);

            while (list.Count < BatchSize && DateTime.UtcNow < timeOutTime)
            {
                if (_logQueue.Count > 0)
                {
                    //lock here so you can enqueue while we are still dequeuing in worker threads
                    lock (_logQueue)
                    {
                        //while we have a lock on the queue, burn threw it till we empty it or fill ourselves up
                        while (list.Count < BatchSize && _logQueue.Count > 0)
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
            if (_logQueue.Count > MaxQueueSize)
            {
                if (IsBlocking)
                {
                    if (MessagesBlocked != null) { MessagesBlocked(this, null); }
                    while (_logQueue.Count > (MaxQueueSize / 2)) // wait for queue to be half empty to prevent full message queue spam
                    {
                        Thread.Sleep(_queueTimeOut);
                    }
                }
                else
                {
                    lock (_logQueue)
                    {
                        _logQueue.Clear();
                        if (MessagesDropped != null) { MessagesDropped(this, null); }
                    }
                }
            }
        }

        #endregion
    }
}
