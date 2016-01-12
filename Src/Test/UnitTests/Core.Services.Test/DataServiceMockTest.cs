using Core.Models.Persistent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Mocks;

namespace Core.Services.Test
{
    [TestClass]
    public class DataServiceMockTest
    {
        private const string _folder = "DataServiceData\\DataServiceMockTest";

        [TestMethod]
        public void DataServiceMockTest_InsertTest()
        {
            var service = BuildMeAMockService();

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(service.Insert(newUser));
        }

        [TestMethod]
        public void DataServiceMockTest_UpdateTest()
        {
            var service = BuildMeAMockService(_folder);

            var user = new User();
            user.UserId = 5;
            user.UserName = "Weeeeee";

            Assert.IsTrue(service.Update(0, user));

            var newUser = service.Find<User>(5);
            Assert.IsNotNull(newUser);
            Assert.AreEqual(user.UserName, newUser.UserName);
            Assert.AreEqual(user.UserId, newUser.UserId);
        }

        [TestMethod]
        public void DataServiceMockTest_FindTest()
        {
            var service = BuildMeAMockService(_folder);

            var user = service.Find<User>(0);

            Assert.IsNotNull(user);
            Assert.AreEqual(0, user.UserId);
            Assert.AreEqual("Bobby", user.UserName);

            var notFound = service.Find<User>(-1);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void DataServiceMockTest_FindWhereTest()
        {
            var service = BuildMeAMockService(_folder);

            var user = service.Find<User>(u => u.UserName == "Bobby");
            var userAlso = service.Find<User>(u => u.UserId == 0);
            var userNotFound = service.Find<User>(u => u.UserName == "NotFound");

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Count);

            var user1 = user.FirstOrDefault();
            Assert.IsNotNull(user1);
            Assert.AreEqual(0, user1.UserId);
            Assert.AreEqual("Bobby", user1.UserName);

            Assert.IsNotNull(userAlso);
            Assert.AreEqual(1, userAlso.Count);

            var user2 = userAlso.FirstOrDefault();
            Assert.IsNotNull(user2);
            Assert.AreEqual(0, user2.UserId);
            Assert.AreEqual("Bobby", user2.UserName);

            Assert.IsNull(userNotFound);
        }

        [TestMethod]
        public void DataServiceMockTest_DeleteTest()
        {
            var service = BuildMeAMockService(_folder);

            var user = service.Find<User>(0);
            Assert.IsNotNull(user);

            Assert.IsTrue(service.Delete(user));

            service.ResetDataSource();
            Assert.IsTrue(service.Delete<User>(0));

            service.ResetDataSource();
            Assert.IsFalse(service.Delete<User>(5));
        }

        [TestMethod]
        public void DataServiceMockTest_DeleteWhereTest()
        {
            var service = BuildMeAMockService(_folder);

            var user = service.Find<User>(0);
            Assert.IsNotNull(user);

            Assert.IsTrue(service.Delete<User>(u => u.UserId == 0));

            service.ResetDataSource();
            Assert.IsTrue(service.Delete<User>(u => u.UserName == "Bobby"));

            service.ResetDataSource();
            Assert.IsTrue(service.Delete<User>(u => u.UserName == "Bobby" && u.UserId == 0));

            service.ResetDataSource();
            Assert.IsTrue(service.Delete<User>(u => u.UserName == "Bobby" && u.UserId != 1));

            service.ResetDataSource();
            Assert.IsFalse(service.Delete<User>(u => u.UserId == 1));
        }

        #region Private Methods

        private DataServiceMock BuildMeAMockService(string folderName = null)
        {
            return folderName == null ? new DataServiceMock() : new DataServiceMock(folderName);
        }

        #endregion
    }
}
