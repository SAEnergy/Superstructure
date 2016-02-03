using Core.Models.Persistent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Test.Mocks;

namespace Core.Components.Test
{
    [TestClass]
    public class DataComponentMockTest
    {
        private const string _folder = "DataComponentData\\DataComponentMockTest";

        [TestMethod]
        public void DataComponentMock_InsertTest()
        {
            var component = BuildMeAMockComponent();

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(component.Insert(newUser));
        }

        [TestMethod]
        public void DataComponentMockTest_UpdateTest()
        {
            var component = BuildMeAMockComponent(_folder);

            component.ResetDataSource();

            var user = new User();
            user.UserId = 1;
            user.UserName = "Weeeeee";

            Assert.IsTrue(component.Update(user));

            var newUser = component.Find<User>(1);
            Assert.IsNotNull(newUser);
            Assert.AreEqual(user.UserName, newUser.UserName);
            Assert.AreEqual(user.UserId, newUser.UserId);
        }

        [TestMethod]
        public void DataComponentMockTest_FindTest()
        {
            var component = BuildMeAMockComponent(_folder);

            var user = component.Find<User>(1);

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.UserId);
            Assert.AreEqual("Bobby", user.UserName);

            var notFound = component.Find<User>(-1);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void DataComponentMockTest_FindWhereTest()
        {
            var component = BuildMeAMockComponent(_folder);

            var user = component.Find<User>(u => u.UserName == "Bobby");
            var userAlso = component.Find<User>(u => u.UserId == 1);
            var userNotFound = component.Find<User>(u => u.UserName == "NotFound");

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Count);

            var user1 = user.FirstOrDefault();
            Assert.IsNotNull(user1);
            Assert.AreEqual(1, user1.UserId);
            Assert.AreEqual("Bobby", user1.UserName);

            Assert.IsNotNull(userAlso);
            Assert.AreEqual(1, userAlso.Count);

            var user2 = userAlso.FirstOrDefault();
            Assert.IsNotNull(user2);
            Assert.AreEqual(1, user2.UserId);
            Assert.AreEqual("Bobby", user2.UserName);

            Assert.IsNull(userNotFound);
        }

        [TestMethod]
        public void DataComponentMockTest_DeleteTest()
        {
            var component = BuildMeAMockComponent(_folder);

            var user = component.Find<User>(1);
            Assert.IsNotNull(user);

            Assert.IsTrue(component.Delete(user));

            component.ResetDataSource();
            Assert.IsTrue(component.Delete<User>(1));

            component.ResetDataSource();
            Assert.IsFalse(component.Delete<User>(5));
        }

        [TestMethod]
        public void DataComponentMockTest_DeleteWhereTest()
        {
            var component = BuildMeAMockComponent(_folder);

            var user = component.Find<User>(1);
            Assert.IsNotNull(user);

            Assert.IsTrue(component.Delete<User>(u => u.UserId == 1));

            component.ResetDataSource();
            Assert.IsTrue(component.Delete<User>(u => u.UserName == "Bobby"));

            component.ResetDataSource();
            Assert.IsTrue(component.Delete<User>(u => u.UserName == "Bobby" && u.UserId == 1));

            component.ResetDataSource();
            Assert.IsTrue(component.Delete<User>(u => u.UserName == "Bobby" && u.UserId != 5));

            component.ResetDataSource();
            Assert.IsFalse(component.Delete<User>(u => u.UserId == 5));
        }

        #region Private Methods

        private DataComponentMock BuildMeAMockComponent(string folderName = null)
        {
            return folderName == null ? new DataComponentMock() : new DataComponentMock(folderName);
        }

        #endregion
    }
}
