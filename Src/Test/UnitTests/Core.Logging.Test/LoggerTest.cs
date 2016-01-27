using Core.Interfaces.Logging;
using Core.Logging.LogDestinations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Logging.Test
{

    [TestClass]
    public class LoggerTest : IDisposable
    {
        private static ILogger _log;
        private static TestLogDestination _dest = new TestLogDestination();
        private Random _randy = new Random();

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _log = Logger.CreateInstance();
            _log.AddLogDestination(_dest);
            _log.Start();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _log.Start();

            Thread.Sleep(1000);

            _dest.Messages.Clear();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _log.Stop();
            _dest.Messages.Clear();
        }

        public void Dispose()
        {
        }

        [TestMethod, Timeout(15000)]
        public void LoggerDeadlockTest()
        {
            _dest.GenerateErrrorsWhileProcessing = true;
            _dest.MaxQueueSize = 1;
            _dest.BatchSize = 1;

            for (int x = 0; x < 10; x++)
            {
                _log.Log(RandomString());
            }
            _log.Flush();

            Assert.IsTrue(_dest.Messages.Count >= 10, "Count is " + _dest.Messages.Count);
        }

        [TestMethod]
        public void LoggerProperBlockingTest()
        {
            int messageCount = 100;
            int sleepTime = 100;

            _dest.GenerateErrrorsWhileProcessing = false;
            _dest.SleepTime = sleepTime;
            _dest.MaxQueueSize = 1;
            _dest.BatchSize = 1;

            Stopwatch time = Stopwatch.StartNew();

            for (int x = 0; x < messageCount; x++)
            {
                _log.Log(RandomString());
            }

            _log.Flush();

            Assert.IsTrue(time.ElapsedMilliseconds >= messageCount * sleepTime);
        }

        [TestMethod]
        public void LoggerOrderingTest()
        {
            _dest.BatchSize = 150;
            _dest.MaxQueueSize = 5000;
            _dest.GenerateErrrorsWhileProcessing = false;

            int messageCount = 10000;
            List<string> messages = new List<string>();

            for (int x = 0; x < messageCount; x++)
            {
                string s = RandomString();
                messages.Add(s);
                _log.Log(s);
            }

            _log.Flush();

            Assert.AreEqual(messages.Count, _dest.Messages.Count);

            for(int x=0; x < messageCount; x++)
            {
                Assert.AreEqual(messages[x], _dest.Messages[x].Message);
            }
        }

        [TestMethod]
        public void LoggerCallerSourceTest()
        {
            _log.Log("hello");
            _log.Flush();
            Assert.AreEqual("LoggerCallerSourceTest", _dest.Messages[0].CallerName);
        }

        private string RandomString(int len = 1024)
        {
            char[] chars = new char[len];
            for(int x=0;x< len; x++)
            {
                chars[x] = (char)(_randy.Next(26) + 'A');
            }
            return new string(chars);
        }

    }

    class TestLogDestination : LogDestinationBase
    {

        public bool GenerateErrrorsWhileProcessing { get; set; }

        public int MaxQueueSize
        {
            get { return _destinationQueue.MaxQueueSize; }
            set { _destinationQueue.MaxQueueSize = value; }
        }

        public int BatchSize
        {
            get { return _destinationQueue.BatchSize; }
            set { _destinationQueue.BatchSize = value; }
        }

        public int SleepTime { get; set; }


        public TestLogDestination()
        {
            _destinationQueue.IsBlocking = true;
        }

        public List<LogMessage> Messages = new List<LogMessage>();

        public override void ReportMessages(List<LogMessage> messages)
        {
            Messages.AddRange(messages);
            if (GenerateErrrorsWhileProcessing && Messages.Count % 2 == 0)
            {
                _logger.HandleLoggingException("It done broke!");
            }
            Thread.Sleep(SleepTime);
        }

        protected override void MessagesBlocked(object sender, EventArgs e)
        {
            // don't generate messages
        }

        protected override void MessagesDropped(object sender, EventArgs e)
        {
            // don't generate messages
        }
    }
}
