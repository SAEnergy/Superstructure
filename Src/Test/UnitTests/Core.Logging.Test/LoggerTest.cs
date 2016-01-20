using Core.Interfaces.Logging;
using Core.Logging.LogDestinations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging.Test
{

    [TestClass]
    public class LoggerTest : IDisposable
    {
        private Logger _log;
        private TestLogDestination _dest = new TestLogDestination();

        public LoggerTest()
        {
            _log = new Logger();
            _log.AddLogDestination(_dest);
            _log.Start();

            Thread.Sleep(1000);
            //_log.Flush();

            _dest.Messages.Clear();
        }

        public void Dispose()
        {
        }

        [TestMethod]
        public void LoggerDeadlockTest()
        {
            for(int x=0;x<100;x++)
            {
                _log.Log("I am the very model of a modern major general.");
            }
            _log.Stop();
            Assert.IsTrue(_dest.Messages.Count > 100);
        }

        [TestMethod]
        public void LoggerOrderingTest()
        {
        }

        [TestMethod]
        public void LoggerCallerSourceTest()
        {
            _log.Log("hello");
            _log.Stop();
            Assert.AreEqual("LoggerCallerSourceTest", _dest.Messages[0].CallerName);
        }

    }

    class TestLogDestination : LogDestinationBase
    {
        public TestLogDestination()
        {
            _destinationQueue.MaxQueueSize = 1;
            _destinationQueue.BatchSize = 1;
            _destinationQueue.IsBlocking = true;
        }

        public List<LogMessage> Messages = new List<LogMessage>();

        public override void ReportMessages(List<LogMessage> messages)
        {
            Messages.AddRange(messages);
            Thread.Sleep(10);
        }
    }
}
