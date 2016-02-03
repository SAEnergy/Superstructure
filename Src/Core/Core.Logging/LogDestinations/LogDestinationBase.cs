using Core.Interfaces.Components.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Core.Logging.LogDestinations
{
    public abstract class LogDestinationBase : ILogDestination
    {
        #region Fields

        protected LogMessageQueue _destinationQueue;
        protected Thread _logDestinationWorkerThread;
        protected ILogger _logger;
        protected ManualResetEvent _queueEmpty = new ManualResetEvent(false);

        private object syncObject = new object();

        #endregion

        #region Properties

        public bool IsRunning { get; protected set; }

        #endregion

        #region Constructor

        protected LogDestinationBase()
        {
            _destinationQueue = new LogMessageQueue() { IsBlocking = true }; //default all log destinations to block
            _destinationQueue.MessagesDropped += MessagesDropped;
            _destinationQueue.MessagesBlocked += MessagesBlocked;
        }

        protected virtual void MessagesBlocked(object sender, EventArgs e)
        {
            _logger.HandleLoggingException(string.Format("This LogDestination's internal queue size of {0} has been overwhelmed.  Waiting for queue to empty.", _destinationQueue.MaxQueueSize));
        }

        protected virtual void MessagesDropped(object sender, EventArgs e)
        {
            _logger.HandleLoggingException(string.Format("This LogDestination's internal queue size of {0} has been overwhelmed.  This has not been configured to block, so messages have been lost.", _destinationQueue.MaxQueueSize));
        }

        #endregion

        #region Public Methods

        public void ProcessMessages(List<LogMessage> messages)
        {
            _queueEmpty.Reset();
            _destinationQueue.EnqueueMessages(messages);
        }

        public void Start()
        {
            lock(syncObject)
            {
                //we want to be able to log internally if something goes wrong
                if (Logger.Instance != null)
                {
                    _logger = Logger.Instance;

                    //_logger.Log(string.Format("LogDestination of type \"{0}\" starting.", this.GetType().Name));

                    if (!IsRunning)
                    {
                        _logDestinationWorkerThread = new Thread(new ThreadStart(LogDestinationWorker));

                        _logDestinationWorkerThread.Start();
                    }
                }
                else
                {
                    throw new NullReferenceException("Logger is null, cannot start log destinations until a logger has been created.");
                }
            }
        }

        public void Stop()
        {
            lock (syncObject)
            {
                if (IsRunning)
                {
                    Flush();

                    IsRunning = false;

                    _logDestinationWorkerThread.Join();

                    ShutDownDestination();
                }
            }
        }

        public abstract void ReportMessages(List<LogMessage> messages);

        public virtual void ShutDownDestination()
        {
            //do nothing is base class, child classes can put shutdown logic in an override method if needed
        }

        #endregion

        #region Private Methods

        private void LogDestinationWorker()
        {
            IsRunning = true;

            while (IsRunning || !_destinationQueue.IsQueueEmpty)
            {
                var messages = _destinationQueue.DequeueMessages();

                if (messages.Count > 0)
                {
                    ReportMessages(messages);
                }

                if (_destinationQueue.IsQueueEmpty) { _queueEmpty.Set(); }
            }
        }

        public void HandleLoggingException(LogMessage message)
        {
            _destinationQueue.HandleLoggingException(message);
        }
        
        public void Flush()
        {
            while(true)
            {
                _queueEmpty.WaitOne(100);
                if(_destinationQueue.IsQueueEmpty) { break; }
            }
        }

        #endregion
    }
}
