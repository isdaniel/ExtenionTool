using NUnit.Framework;
using ExtensionTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionTool.Tests
{
    public class Tag1 : Attribute
    {
        public Tag1(string v)
        {
            Value = v;
        }

        public string Value { get; set; }
    }

    [Tag1("1234")]
    public class WithTag { }

    public class NoTag { }

    [TestFixture()]
    public class ExtensionAttributeTests
    {
        [Test()]
        public void Test_WithTagCurrectValue_True()
        {
            string recept = "1234";

            string result = typeof(WithTag).GetAttributeValue((Tag1 tag) => tag.Value);

            Assert.AreEqual(result, recept);
        }

        [Test()]
        public void Test_NOTage_True()
        {
            string recpt = default(string);

            string result = typeof(NoTag).GetAttributeValue((Tag1 tag) => tag.Value);

            Assert.AreEqual(result, recpt);
        }
    }
}