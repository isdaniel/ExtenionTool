using NUnit.Framework;
using ExtensionTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExtensionTool.Tests
{
    public class DisplayAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public enum e_Type
    {
        [Display(Name = nameof(A))]
        A = 1,

        [Display(Name = nameof(B))]
        B = 2,

        [Display(Name = nameof(C))]
        C = 4
    }

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

        [Test]
        [TestCaseSource("GetEnumTest")]
        public void CheckAttribute_True(TestEnum tester)
        {
            tester.Test();
        }

        [TestCase(e_Type.A, "A")]
        [TestCase(e_Type.B, "B")]
        [TestCase(e_Type.C, "C")]
        public void GetAttributeValue_ByEnum_True([Values]e_Type enumVal, string except)
        {
            string result = enumVal.GetAttributeValue((DisplayAttribute attr) => attr.Name);

            Assert.AreEqual(result, except);
        }

        [Test]
        public void GetAttributeValue_ByEnum_Exception()
        {
            int num = 1;

            var result = Assert.Throws<ArgumentException>(() => { num.GetAttributeValue((DisplayAttribute attr) => attr.Name); });
            Assert.AreEqual(result.Message, "TEnum must be an enum!!");
        }

        public static IEnumerable<TestEnum> GetEnumTest()
        {
            yield return new TestEnum()
            {
                Except = nameof(e_Type.A),
                type = e_Type.A,
                Value = 1
            };
            yield return new TestEnum()
            {
                Except = nameof(e_Type.B),
                type = e_Type.B,
                Value = 2
            };
            yield return new TestEnum()
            {
                Except = nameof(e_Type.C),
                type = e_Type.C,
                Value = 4
            };
        }
    }

    public class TestEnum
    {
        public e_Type type { get; set; }
        public string Except { get; set; }

        public int Value { get; set; }

        public void Test()
        {
            string result = typeof(e_Type).GetField(type.ToString())
                .GetAttributeValue((DisplayAttribute attr) => attr.Name);

            Assert.AreEqual(Value, (int)typeof(e_Type).GetField(type.ToString()).GetValue(type));
            Assert.AreEqual(Except, result);
        }
    }
}