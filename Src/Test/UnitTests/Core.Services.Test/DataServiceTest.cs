using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Database;
using Test.Mocks;
using Core.Interfaces.Logging;
using Core.Models.Persistent;
using System.Data.Entity.Core.EntityClient;

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
        public void InsertTest()
        {
            var service = BuildMeADaService();

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(service.Insert<User>(newUser));
        }

        [TestMethod]
        public void UpdateTest()
        {
        }

        [TestMethod]
        public void FindTest()
        {
        }

        [TestMethod]
        public void FindWhereTest()
        {
        }

        [TestMethod]
        public void DeleteTest()
        {
        }

        [TestMethod]
        public void DeleteWhereTest()
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
