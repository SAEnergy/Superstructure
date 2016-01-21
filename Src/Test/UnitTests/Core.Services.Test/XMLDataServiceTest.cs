using Core.Interfaces.Services;
using Core.Models.Persistent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using Test.Mocks;

namespace Core.Services.Test
{
    [TestClass]
    public class XMLDataServiceTest
    {
        private static IDataService _service = XMLDataService.CreateInstance(new LoggerMock());

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            string timeStamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);

            string fileName = string.Format("RunTime_{0}.xml", timeStamp);

            File.Copy("XMLTestData\\DataService.xml", fileName);

            XMLDataService.Folder = Environment.CurrentDirectory;
            XMLDataService.FileName = fileName;
        }

        [TestMethod]
        public void XMLDataServiceTest_InsertTest()
        {
            var newUser = new User();
            newUser.UserName = "Bobby";

            Assert.IsTrue(_service.Insert(newUser));

            var realNewUser = _service.Find<User>(1000);
            Assert.IsNotNull(realNewUser);
            Assert.AreEqual("Bobby", realNewUser.UserName);
        }

        [TestMethod]
        public void XMLDataServiceTest_DeleteTest()
        {
            var newUser = new User();
            newUser.UserId = 1;
            newUser.UserName = "Bobby";

            Assert.IsTrue(_service.Delete(newUser));
            var realNewUser = _service.Find<User>(1);
            Assert.IsNull(realNewUser);
        }

        [TestMethod]
        public void XMLDataServiceTest_DeleteWhereTest()
        {
            Assert.IsTrue(_service.Delete<User>(x => x.UserName == "BobbyDeleteWhere"));

            var realUser = _service.Find<User>(3);
            Assert.IsNull(realUser);
        }

        [TestMethod]
        public void XMLDataServiceTest_DeleteKeyTest()
        {
            Assert.IsTrue(_service.Delete<User>(2));

            var realUser = _service.Find<User>(2);
            Assert.IsNull(realUser);
        }

        [TestMethod]
        public void XMLDataServiceTest_FindKeyTest()
        {
            var user = _service.Find<User>(999);

            Assert.IsNotNull(user);
            Assert.AreEqual(999, user.UserId);
            Assert.AreEqual("BobbyInsertAfter", user.UserName);
        }

        [TestMethod]
        public void XMLDataServiceTest_FindWhereTest()
        {
            var list = _service.Find<User>(u => u.UserName == "BobbyInsertAfter");

            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);

            var realuser = list.FirstOrDefault();

            Assert.IsNotNull(realuser);
            Assert.AreEqual(999, realuser.UserId);
            Assert.AreEqual("BobbyInsertAfter", realuser.UserName);
        }

    }
}
