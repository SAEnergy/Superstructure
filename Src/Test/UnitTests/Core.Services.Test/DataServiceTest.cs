using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Database;
using Test.Mocks;
using Core.Interfaces.Logging;
using Core.Models.Persistent;

namespace Core.Services.Test
{
    [TestClass]
    public class DataServiceTest
    {
        [TestInitialize]
        public void Init()
        {
            DatabaseSettings.Instance.ConnectionString = string.Format("Data Source={0}\\HostServiceTestDb.sdf", Environment.CurrentDirectory);
        }

        [TestMethod]
        public void DataServiceTest_InsertTest()
        {
            var service = BuildMeADaService();

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(service.Insert<User>(newUser));
        }

        [TestMethod]
        public void DataServiceTest_UpdateTest()
        {
        }

        [TestMethod]
        public void DataServiceTest_FindTest()
        {
        }

        [TestMethod]
        public void DataServiceTest_FindWhereTest()
        {
        }

        [TestMethod]
        public void DataServiceTest_DeleteTest()
        {
        }

        [TestMethod]
        public void DataServiceTest_DeleteWhereTest()
        {
        }

        #region Private Methods

        private DataService BuildMeADaService()
        {
            ILogger mockLogger = new LoggerMock();

            return new DataService(mockLogger);
        }

        #endregion
    }
}
