using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using NUnit.Framework;
using ThirdPartyExtension;

namespace ThirdPartyExtensionTests
{
    [Intercept(typeof(LockInterceptor))]
    public partial class LockerContext : ILockerContext
    {
        [LockBy(Key = "LockKey")]
        public virtual void MethodA ()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodA");
            Thread.Sleep(100);
        }

        [LockBy(Key = "LockKey")]
        public virtual  void MethodA1()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodA1");
            Thread.Sleep(100);
        }

        [LockBy(Key = "LockKey")]
        public virtual  void MethodA2()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodA2");
            Thread.Sleep(100);
        }

        [LockBy(Key = "LockKey")]
        public virtual  void MethodB()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss fff} MethodB");
            Thread.Sleep(100);
        }
    }

    public interface ILockerContext
    {
        void MethodA();
        void MethodA1();
        
        void MethodA2();
        
        void MethodB();
    }

    public class AutofacConfig
    {
        public static IContainer Container { get; set; }

        public static void Register()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<LockInterceptor>().AsSelf();
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
                lockerContext.MethodA1();
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodA(); }));
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodA1(); }));
                taskList.Add(Task.Factory.StartNew(() => { lockerContext.MethodB(); }));
                
            }

            Task.WaitAll(taskList.ToArray());

        }
    }
}
