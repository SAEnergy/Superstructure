using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Logging;
using Core.Interfaces.Logging;
using System.Runtime.CompilerServices;

namespace Core.Logging.Test
{
    [TestClass]
    public class LoggingTest
    {
        [TestMethod]
        public void TestUserExtensibleEnum()
        {
            Assert.AreEqual(6400, LogMessageSeverity.Critical.Code, "Default Code Resolves Correctly");
            Assert.AreEqual("Critical", LogMessageSeverity.Critical.ToString(), "Default Name Resolves Correctly");
            Assert.AreEqual(999, CustomLogMessageSeverity.George.Code, "Custom Code Resolves Correctly");
            Assert.AreEqual("George", CustomLogMessageSeverity.George.ToString(), "Custom Name Resolves Correctly");

            Assert.IsTrue(LogMessageSeverity.Critical > CustomLogMessageSeverity.George, "Can Compare");

            // :(

            //LogMessageSeverity val = CustomLogMessageSeverity.George;
            //switch (val)
            //{
            //    case LogMessageSeverity.Critical.Code: break;
            //    case CustomLogMessageSeverity.George: break;
            //    default: Assert.Fail("This shouldn't happen."); break;
            //}
        }
    }

    public class CustomLogMessageSeverity : LogMessageSeverity
    {
        public static LogMessageSeverity George = new LogMessageSeverity(999);

        public CustomLogMessageSeverity(int code, [CallerMemberName] string name = "") : base(code,name) { }
    }

}
