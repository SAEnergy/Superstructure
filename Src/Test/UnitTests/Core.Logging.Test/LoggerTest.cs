using Core.Interfaces.Logging;
using Core.Logging.LogDestinations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Test.Helpers;

namespace Core.Logging.Test
{

    [TestClass]
    public class LoggerTest
    {
        private Random _randy = new Random();

        [TestInitialize]
        public void InitializeTest()
        {
            SingletonHelper.Clean(typeof(Logger));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SingletonHelper.Clean(typeof(Logger));
        }

        [TestMethod, Timeout(30000)]
        public void LoggerDeadlockTest()
        {
            var logTup = CreateLogger();

            logTup.Item2.GenerateErrrorsWhileProcessing = true;
            logTup.Item2.MaxQueueSize = 1;
            logTup.Item2.BatchSize = 1;

            for (int x = 0; x < 10; x++)
            {
                logTup.Item1.Log(RandomString());
            }

            logTup.Item1.Flush();

            Thread.Sleep(1000);

            Assert.IsTrue(logTup.Item2.Messages.Count >= 10, "Count is " + logTup.Item2.Messages.Count);

            logTup.Item1.Stop();
        }

        [TestMethod]
        public void LoggerProperBlockingTest()
        {
            var logTup = CreateLogger();

            int messageCount = 100;
            int sleepTime = 100;

            logTup.Item2.GenerateErrrorsWhileProcessing = false;
            logTup.Item2.SleepTime = sleepTime;
            logTup.Item2.MaxQueueSize = 1;
            logTup.Item2.BatchSize = 1;

            Stopwatch time = Stopwatch.StartNew();

            for (int x = 0; x < messageCount; x++)
            {
                logTup.Item1.Log(RandomString());
            }

            logTup.Item1.Flush();

            Thread.Sleep(1000);

            Assert.IsTrue(time.ElapsedMilliseconds >= messageCount * sleepTime);

            logTup.Item1.Stop();
        }

        [TestMethod]
        public void LoggerOrderingTest()
        {
            var logTup = CreateLogger();

            logTup.Item2.BatchSize = 150;
            logTup.Item2.MaxQueueSize = 5000;
            logTup.Item2.GenerateErrrorsWhileProcessing = false;

            int messageCount = 10000;
            List<string> messages = new List<string>();

            logTup.Item2.Messages.Clear();

            for (int x = 0; x < messageCount; x++)
            {
                string s = RandomString();
                messages.Add(s);
                logTup.Item1.Log(s);
            }

            logTup.Item1.Flush();

            Thread.Sleep(1000);

            Assert.AreEqual(messages.Count, logTup.Item2.Messages.Count);

            for(int x=0; x < messageCount; x++)
            {
                Assert.AreEqual(messages[x], logTup.Item2.Messages[x].Message);
            }

            logTup.Item1.Stop();
        }

        [TestMethod]
        public void LoggerCallerSourceTest()
        {
            var logTup = CreateLogger();

            logTup.Item1.Log("hello");
            logTup.Item1.Flush();

            Thread.Sleep(1000);

            Assert.AreEqual("LoggerCallerSourceTest", logTup.Item2.Messages[0].CallerName);

            logTup.Item1.Stop();
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

        private Tuple<ILogger, TestLogDestination> CreateLogger()
        {
            ILogger logger = Logger.CreateInstance();
            TestLogDestination dest = new TestLogDestination();
            logger.AddLogDestination(dest);

            logger.Start();

            logger.Flush();

            Thread.Sleep(1000);

            dest.Messages.Clear();

            return new Tuple<ILogger, TestLogDestination>(logger, dest);
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
