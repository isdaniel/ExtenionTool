using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.Core.Internal;
using NUnit.Framework;
using ThirdPartyExtension;

namespace ThirdPartyExtensionTests
{
    [Intercept(typeof(LockerInterceptor))]
    public class LockerContext : ILockerContext
    {
        [Lock(Key = "A", Mode = LockMode.SharedLock)]
        public virtual void MethodA ()
        {
            Thread.Sleep(5);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodA Done");
            Thread.Sleep(5);
        }

        [Lock(Key = "A")]
        public virtual void MethodA1()
        {
            Thread.Sleep(5);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodA1 Done");
            Thread.Sleep(5);
        }

        [Lock(Key = "A")]
        [Lock(Key = "B")]
        public virtual void MethodB_A()
        {
            Thread.Sleep(5);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodB_A Done");
            Thread.Sleep(5);
        }

        [Lock(Key = "B")]
        public virtual  void MethodB()
        {
            Thread.Sleep(5);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodB Done");
            Thread.Sleep(5);
        }
    }

    public interface ILockerContext
    {
        void MethodA();
        void MethodA1();
        
        void MethodB_A();
        
        void MethodB();
    }

    public class AutofacConfig
    {
        public static IContainer Container { get; set; }

        public static void Register()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<LockerInterceptor>().AsSelf();
            builder.RegisterType<LockerContext>().As<ILockerContext>().EnableClassInterceptors();
            builder.RegisterType<ConsoleProvider>().As<ISysLog>().SingleInstance();
            Container = builder.Build();
        }
    }

    [TestFixture]
    public class LockerTest
    {
        //You can use the following additional attributes as you write your tests:
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AutofacConfig.Register();
        }

        //Use OneTimeTearDown to run code after all tests in a class have run
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        //Use SetUp to run code before running each test
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            
        }


        [Test]
        public void LockGroupTest()
        { 
            var lockerContext= AutofacConfig.Container.Resolve<ILockerContext>();
            
            List<Task> taskList =new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodA(); }));
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodA1(); }));
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodB_A(); }));                
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodB(); }));

            }

            Task.WaitAll(taskList.ToArray());

        }
    }
}
