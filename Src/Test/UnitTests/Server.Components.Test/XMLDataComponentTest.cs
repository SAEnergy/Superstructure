using Server.Components;
using Core.Models.Persistent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Test.Mocks;

namespace Core.Components.Test
{
    [TestClass]
    public class XMLDataComponentTest
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            string timeStamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.UtcNow);

            string fileName = string.Format("RunTime_{0}.xml", timeStamp);

            File.Copy("XMLTestData\\DataComponent.xml", fileName);

            XMLDataComponent.Folder = Environment.CurrentDirectory;
            XMLDataComponent.FileName = fileName;
        }

        [TestMethod]
        public void XMLDataComponentTest_InsertTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(component.Insert(newUser));

            var realNewUser = component.Find<User>(1000);
            Assert.IsNotNull(realNewUser);
            Assert.AreEqual("Bobby", realNewUser.UserName);
        }

        [TestMethod]
        public void XMLDataComponentTest_DeleteTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            var newUser = new User();
            newUser.UserId = 1;
            newUser.UserName = "Bobby";

            Assert.IsTrue(component.Delete(newUser));
            var realNewUser = component.Find<User>(1);
            Assert.IsNull(realNewUser);
        }

        [TestMethod]
        public void XMLDataComponentTest_DeleteWhereTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            Assert.IsTrue(component.Delete<User>(x => x.UserName == "BobbyDeleteWhere"));

            var realUser = component.Find<User>(3);
            Assert.IsNull(realUser);
        }

        [TestMethod]
        public void XMLDataComponentTest_DeleteKeyTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            Assert.IsTrue(component.Delete<User>(2));

            var realUser = component.Find<User>(2);
            Assert.IsNull(realUser);
        }

        [TestMethod]
        public void XMLDataComponentTest_FindKeyTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            var user = component.Find<User>(999);

            Assert.IsNotNull(user);
            Assert.AreEqual(999, user.UserId);
            Assert.AreEqual("BobbyInsertAfter", user.UserName);
        }

        [TestMethod]
        public void XMLDataComponentTest_FindWhereTest()
        {
            var component = new XMLDataComponent(new LoggerMock());

            var list = component.Find<User>(u => u.UserName == "BobbyInsertAfter");

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);

            var realuser = list.FirstOrDefault();

            Assert.IsNotNull(realuser);
            Assert.AreEqual(999, realuser.UserId);
            Assert.AreEqual("BobbyInsertAfter", realuser.UserName);
        }

    }
}
