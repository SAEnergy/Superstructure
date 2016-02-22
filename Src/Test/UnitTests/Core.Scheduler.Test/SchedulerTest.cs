using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Mocks;
using Core.Models.Persistent;
using Test.Helpers;
using Test.Plugins.Mocks;
using System.Threading;
using System.Linq;

namespace Core.Scheduler.Test
{
    [TestClass]
    public class SchedulerTest
    {
        [TestInitialize]
        public void InitializeTest()
        {
            SingletonHelper.Clean(typeof(SchedulerComponent));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SingletonHelper.Clean(typeof(SchedulerComponent));
        }

        [TestMethod]
        [Ignore]
        public void SchedulerTest_AddCustomJob()
        {
            var scheduler = SchedulerComponent.CreateInstance(new LoggerMock(), new DataComponentMock());

            scheduler.Start();

            scheduler.AddJob(BuildMeAJob());

            //give the scheduler time to create the instance and run it
            while(UnitTestJob.Instances == null)
            {
                Thread.Sleep(100);
            }

            var job = UnitTestJob.Instances.FirstOrDefault();

            Assert.IsNotNull(job);

            scheduler.Stop();
        }

        private JobConfiguration BuildMeAJob()
        {
            var config = new JobConfiguration();

            config.ActionType = "UnitTestJob";

            return config;
        }
    }
}
