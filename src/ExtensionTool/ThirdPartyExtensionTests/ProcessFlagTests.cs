//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Autofac;
//using Autofac.Extras.DynamicProxy;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using ThirdPartyExtension;

//namespace ThirdPartyExtensionTests
//{
//    [Intercept(typeof(LockInterceptor))]
//    public partial  class LockerContext
//    {

//        [LockBy(Key = "Lock")]
//        public virtual void Locker1(DateTime? dt)
//        {
      
//            Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff}  Locker1 {dt:MM/dd/yyyy HH:mm:ss fff}");
//        }

//        [LockBy(Key = "Lock")]
//        public virtual void Locker2(DateTime? dt)
//        {
//            Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff}  Locker2  {dt:MM/dd/yyyy HH:mm:ss fff}");
//        }
//    }

//    [TestClass()]
//    public class ProcessFlagTests
//    {
//        private IContainer _container;
//        [TestInitialize]
//        public void Init()
//        {
//            ContainerBuilder builder = new ContainerBuilder();
//            builder.RegisterType<LockInterceptor>().AsSelf();
//            builder.RegisterType<LockerContext>().EnableClassInterceptors();
//            builder.RegisterType<ConsoleProvider>().As<ISysLog>().SingleInstance();
//            _container = builder.Build();
//        }


//        [TestMethod()]
//        public void LockInterceptorTest()
//        {
//            var lockerContext = _container.Resolve<LockerContext>();

//            var task1 = Task.Run(() =>
//            {
//                for (int i = 0; i < 100; i++)
//                {
//                    lockerContext.Locker1(DateTime.Now);
//                    Thread.Sleep(10);
//                }
//            });
//            var task2 = Task.Run(() =>
//            {
//                for (int i = 0; i < 100; i++)
//                {
//                    lockerContext.Locker2(DateTime.Now);
//                    Thread.Sleep(10);
//                }
//            });

//            Task.WaitAll(task1, task2);
//        }
//    }
//}