using Core.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging.LogDestinations
{
    public abstract class BaseLogDestination : ILogDestination
    {
        #region Fields

        protected Queue<LogMessage> _destinationQueue;
        protected Thread _logDestinationWorkerThread;

        #endregion

        #region Properties

        public bool IsRunning { get; protected set; }

        #endregion

        #region Constructor

        protected BaseLogDestination()
        {
            _destinationQueue = new Queue<LogMessage>();
            _logDestinationWorkerThread = new Thread(new ThreadStart(LogDestinationWorker));
            _logDestinationWorkerThread.IsBackground = true;
        }

        #endregion

        #region Public Methods

        public void ProcessMessages(List<LogMessage> messages)
        {
            lock(_destinationQueue)
            {
                foreach (var message in messages)
                {
                    _destinationQueue.Enqueue(message);
                }
            }
        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public abstract void ProcessMessage(LogMessage message);

        #endregion

        #region Private Methods

        private void LogDestinationWorker()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
