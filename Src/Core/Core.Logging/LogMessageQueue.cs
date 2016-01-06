using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging
{
    public class LogMessageQueue
    {
        #region Fields

        private TimeSpan _queueTimeOut = new TimeSpan(0, 0, 0, 0, 100); // milliseconds
        private const int _queueBatchSize = 50;

        private Queue<LogMessage> _logQueue;

        #endregion

        #region Constructor

        public LogMessageQueue()
        {
            _logQueue = new Queue<LogMessage>();
        }

        #endregion

        #region Public Methods

        public void EnqueueMessages(List<LogMessage> messages)
        {
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
            lock (_logQueue)
            {
                _logQueue.Enqueue(message);
            }
        }

        public List<LogMessage> DequeueMessages()
        {
            var list = new List<LogMessage>();
            var sleepTime = _queueTimeOut.Milliseconds / _queueBatchSize;
            var timeOutTime = DateTime.UtcNow.Add(_queueTimeOut);

            while (list.Count < _queueBatchSize && DateTime.UtcNow < timeOutTime)
            {
                if (_logQueue.Count > 0)
                {
                    //lock here so you can enqueue while we are still dequeuing in worker threads
                    lock (_logQueue)
                    {
                        list.Add(_logQueue.Dequeue());
                    }
                }

                Thread.Sleep(sleepTime);
            }

            return list;
        }

        #endregion
    }
}
