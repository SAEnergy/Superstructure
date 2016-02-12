using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Util.Test
{
    [TestClass]
    public class PascalCaseSplitterTest
    {
        [TestMethod]
        public void PascalCaseSplitterTestNumbers()
        {
            Assert.AreEqual("Foo Bar 2000", PascalCaseSplitter.Split("FooBar2000"));
            Assert.AreEqual("Frank's 2000 Inch TV", PascalCaseSplitter.Split("Frank's2000InchTV"));
            Assert.AreEqual("ASDF 8000 WEE", PascalCaseSplitter.Split("ASDF8000WEE"));
            Assert.AreEqual("9000", PascalCaseSplitter.Split("9000"));
            Assert.AreEqual("Param 9000", PascalCaseSplitter.Split("param9000"));
            Assert.AreEqual("9 Params", PascalCaseSplitter.Split("9params"));
            Assert.AreEqual("Thing Number 2", PascalCaseSplitter.Split("ThingNumber2"));
        }

        [TestMethod]
        public void PascalCaseSplitterTestAcronyms()
        {
            Assert.AreEqual("XML Serializer", PascalCaseSplitter.Split("XMLSerializer"));
            Assert.AreEqual("Serailize Me Some XML", PascalCaseSplitter.Split("SerailizeMeSomeXML"));
            Assert.AreEqual("Do Some XML Stuff", PascalCaseSplitter.Split("DoSomeXMLStuff"));
        }

        [TestMethod]
        public void PascalCaseSplitterTestPascalCase()
        {
            Assert.AreEqual("Pascal", PascalCaseSplitter.Split("Pascal"));
            Assert.AreEqual("Pascal Case Splitter Test Pascal Case", PascalCaseSplitter.Split("PascalCaseSplitterTestPascalCase"));
            Assert.AreEqual("Do Re Me Fa So La Ti Do", PascalCaseSplitter.Split("DoReMeFaSoLaTiDo"));
        }

        [TestMethod]
        public void PascalCaseSplitterTestCamelCase()
        {
            Assert.AreEqual("Camel", PascalCaseSplitter.Split("camel"));
            Assert.AreEqual("Camel Case Splitter", PascalCaseSplitter.Split("camelCaseSplitter"));
        }
    }
}
