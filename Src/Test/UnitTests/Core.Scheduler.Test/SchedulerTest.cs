using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Mocks;

namespace Core.Scheduler.Test
{
    [TestClass]
    public class SchedulerTest
    {
        [TestMethod]
        public void SchedulerTest_RegisterCustomJobActionType()
        {
            var scheduler = SchedulerComponent.CreateInstance(new LoggerMock(), new DataComponentMock());

            Assert.IsTrue(scheduler.RegisterCustomJobActionType("Weeee", typeof(UnitTestJob)));
        }
    }
}
