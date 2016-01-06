using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public abstract class LogDestinationBase : ILogDestination
    {
        #region Fields

        protected LogMessageQueue _destinationQueue;
        protected Thread _logDestinationWorkerThread;
        protected ManualResetEvent _logDestinationDone;

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
                    _logDestinationDone = new ManualResetEvent(false);
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

                    _logDestinationDone.WaitOne();
                }
            }
        }

        public abstract void ProcessMessage(LogMessage message);

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

            _logDestinationDone.Set();
        }

        #endregion
    }
}
