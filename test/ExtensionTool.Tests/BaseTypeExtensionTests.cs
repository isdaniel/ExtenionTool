using ExtensionTool;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;


namespace ExtensionTool.Tests
{
    [TestFixture()]
    public class BaseTypeExtensionTests
    {
        [TestCase()]
        public void object_False()
        {
            object o = new object();

            bool expected = false;

            bool act = o.IsCollectionType();

            Assert.AreEqual(expected, act);
        }

        [TestCase]
        public void Array_True()
        {
            object[] o = { };

            bool expected = true;

            bool act = o.IsCollectionType();

            Assert.AreEqual(expected, act);
        }

        [TestCase]
        public void List_True()
        {
            var o = new List<object>();

            bool expected = true;

            bool act = o.IsCollectionType();

            Assert.AreEqual(expected, act);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NULL_ThrowException()
        {
            object o = null;

            bool expected = true;

            o.IsCollectionType();
        }
    }
}