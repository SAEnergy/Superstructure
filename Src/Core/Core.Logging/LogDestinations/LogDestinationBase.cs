using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Logging.LogDestinations
{
    public abstract class LogDestinationBase : ILogDestination
    {
        #region Fields

        protected LogMessageQueue _destinationQueue;
        protected Thread _logDestinationWorkerThread;
        protected ILogger _logger;

        private object syncObject = new object();

        #endregion

        #region Properties

        public bool IsRunning { get; protected set; }

        #endregion

        #region Constructor

        protected LogDestinationBase()
        {
            _destinationQueue = new LogMessageQueue() { IsBlocking = true }; //default all log destinations to block
        }

        #endregion

        #region Public Methods

        public void ProcessMessages(List<LogMessage> messages)
        {
            _destinationQueue.EnqueueMessages(messages);
        }

        public void Start()
        {
            lock(syncObject)
            {
                //we want to be able to log internally if something goes wrong
                if (Logger.InternalLogger != null)
                {
                    _logger = Logger.InternalLogger;

                    _logger.Log(string.Format("LogDestination of type \"{0}\" starting.", this.GetType().Name));

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
            }
        }

        #endregion
    }
}
