using ExtensionTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Reflection;

namespace ExtensionTool.Tests
{
    public interface MyInterface { }

    public abstract class ClassBase { }

    public class ClassA : ClassBase { }

    public class ClassB : ClassBase { }

    [TestFixture]
    public class AssemblyExtensionTests
    {

        [Test]
        public void GetAll_InheritClassBase_GetbyCurrentAssembly_TwoClasses()
        {
            AssertClassBase(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void GetAll_InheritClassBase_GetbyPath_TwoClasses()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            AssertClassBase(Assembly.LoadFile(assemblyPath));
        }

        [Test]
        public void IsNormalClass_True() {

            Assert.IsTrue(typeof(ClassA).IsNormalClass());
            Assert.IsFalse(typeof(ClassBase).IsNormalClass());
            Assert.IsFalse(typeof(MyInterface).IsNormalClass());
        }

        private void AssertClassBase(Assembly ass) {

            var act = ass.GetInstancesByAssembly<ClassBase>();

            var expect = new List<ClassBase>() {
                new ClassA(),
                new ClassB()
            };

            act.IsInstanceOfAll();
            Assert.IsTrue(act.Count() == 2);
        }
    }


    public static class TestLibExtension {
        public static void IsInstanceOfAll<TType>(this IEnumerable<TType> source) {
            foreach (var item in source)
            {
                Assert.IsInstanceOf<ClassBase>(item);
            }
        }
    }
}