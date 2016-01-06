using Core.Interfaces.Logging;
using System.Collections.Generic;
using System.Threading;

namespace Core.Logging.LogDestinations
{
    public abstract class LogDestinationBase : ILogDestination
    {
        #region Fields

        protected LogMessageQueue _destinationQueue;
        protected Thread _logDestinationWorkerThread;

        private object syncObject = new object();

        #endregion

        #region Properties

        public bool IsRunning { get; protected set; }

        #endregion

        #region Constructor

        protected LogDestinationBase()
        {
            _destinationQueue = new LogMessageQueue();
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
                if(!IsRunning)
                {
                    _logDestinationWorkerThread = new Thread(new ThreadStart(LogDestinationWorker));
                    _logDestinationWorkerThread.IsBackground = true;

                    _logDestinationWorkerThread.Start();

                    IsRunning = true;
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

        public abstract void ProcessMessage(LogMessage message);

        public virtual void ShutDownDestination()
        {
            //do nothing is base class, child classes can put shutdown logic in an override method if needed
        }

        #endregion

        #region Private Methods

        private void LogDestinationWorker()
        {
            while(IsRunning)
            {
                var messages = _destinationQueue.DequeueMessages();

                foreach(var message in messages)
                {
                    ProcessMessage(message);
                }
            }
        }

        #endregion
    }
}
