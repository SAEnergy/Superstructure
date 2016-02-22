using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Mocks;
using Core.Models.Persistent;

namespace Core.Scheduler.Test
{
    [TestClass]
    public class SchedulerTest
    {
        //[TestMethod]
        //public void SchedulerTest_AddCustomJob()
        //{
        //    var scheduler = SchedulerComponent.CreateInstance(new LoggerMock(), new DataComponentMock());

        //    scheduler.AddJob(BuildMeAJob());
        //}

        private JobConfiguration BuildMeAJob()
        {
            var config = new JobConfiguration();

            config.ActionType = "UnitTestJob";

            return config;
        }
    }
}
