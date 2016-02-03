using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Database;
using Test.Mocks;
using Core.Interfaces.Logging;
using Core.Models.Persistent;
using Core.Interfaces.Components;
using System.Linq;
using Core.Components;

namespace Core.Components.Test
{
    [TestClass]
    public class SQLDataComponentTest
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            DatabaseSettings.Instance.ConnectionString = string.Format("Data Source={0}\\HostServiceTestDb.sdf", Environment.CurrentDirectory);
        }

        [TestMethod]
        public void SQLDataComponentTest_InsertTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(component.Insert<User>(newUser));
        }

        [TestMethod]
        public void SQLDataComponentTest_UpdateTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            InsertIfNeeded(component);

            var result = component.Find<User>(u => u.UserName == "Bobby");
            Assert.IsNotNull(result);

            var user = result.FirstOrDefault();
            Assert.IsNotNull(user);
            string oldName = user.UserName;
            user.UserName = "Weeee" + Guid.NewGuid(); //Allows unit test to run over and over again without failing

            Assert.IsTrue(component.Update(user));

            var newUser = component.Find<User>(u => u.UserName == user.UserName).FirstOrDefault();
            Assert.IsNotNull(newUser);
            Assert.AreEqual(user.UserId, newUser.UserId);
            Assert.AreEqual(user.UserName, newUser.UserName);

            var notFound = component.Find<User>(u => u.UserName == oldName);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void SQLDataComponentTest_FindTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            InsertIfNeeded(component);

            var result = component.Find<User>(u => u.UserName == "Bobby");
            Assert.IsNotNull(result);

            var user = result.FirstOrDefault();
            Assert.IsNotNull(user);

            var user1 = component.Find<User>(user.UserId);

            Assert.IsNotNull(user1);
            Assert.AreEqual(user.UserId, user1.UserId);
            Assert.AreEqual(user.UserName, user1.UserName);
        }

        [TestMethod]
        public void SQLDataComponentTest_FindWhereTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            InsertIfNeeded(component);

            var user = component.Find<User>(u => u.UserName == "Bobby");

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Count);

            var realUser = user.FirstOrDefault();
            Assert.IsNotNull(realUser);
            Assert.AreEqual("Bobby", realUser.UserName);

            var userToo = component.Find<User>(u => u.UserId == -1);
            Assert.IsNull(userToo);
        }

        [TestMethod]
        public void SQLDataComponentTest_DeleteTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            InsertIfNeeded(component);

            var user = component.Find<User>(u => u.UserName == "Bobby");

            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Count);

            var bobby = user.FirstOrDefault();
            Assert.AreEqual("Bobby", bobby.UserName);

            Assert.IsTrue(component.Delete<User>(bobby.UserId));
            var find = component.Find<User>(bobby.UserId);

            Assert.IsNull(find);
        }

        [TestMethod]
        public void SQLDataComponentTest_DeleteWhereTest()
        {
            var component = new SQLDataComponent(new LoggerMock());

            InsertIfNeeded(component);

            Assert.IsTrue(component.Delete<User>(u => u.UserName == "Bobby"));

            Assert.IsNull(component.Find<User>(u => u.UserName == "Bobby"));
        }

        #region Private Methods

        private void InsertIfNeeded(IDataComponent component)
        {
            if (component.Find<User>(u => u.UserName == "Bobby") == null)
            {
                var newUser = new User();
                newUser.UserName = "Bobby";

                Assert.IsTrue(component.Insert<User>(newUser));
            }
        }

        #endregion
    }
}
