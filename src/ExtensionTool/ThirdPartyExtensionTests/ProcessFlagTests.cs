﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThirdPartyExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;

namespace ThirdPartyExtension.Tests
{
    [Intercept(typeof(LockInterceptor))]
    public class LockerContext
    {

        [LockBy(Key = "Lock")]
        public virtual void Locker1()
        {
      
            Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff}  Locker1");
        }

        [LockBy(Key = "Lock")]
        public virtual void Locker2()
        {
            Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff}  Locker2");
        }
    }

    [TestClass()]
    public class ProcessFlagTests
    {
        private IContainer _container;
        [TestInitialize]
        public void Init()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<LockInterceptor>().AsSelf();
            builder.RegisterType<ThreadLocker>().SingleInstance();
            builder.RegisterType<LockerContext>().EnableClassInterceptors();
            builder.RegisterType<ConsoleProvider>().As<ISysLog>().SingleInstance();
            _container = builder.Build();
        }

        [TestMethod()]
        public void StopProcessTest()
        {
            ProcessFlag flag = new ProcessFlag();
            flag.StopProcess();
            Assert.IsFalse(flag.IsProcessing);
        }

        [TestMethod()]
        public void LockInterceptorTest()
        {
            var lockerContext = _container.Resolve<LockerContext>();

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    lockerContext.Locker1();
                    Thread.Sleep(1);
                }
            });
            var task2 = Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    lockerContext.Locker2();
                    Thread.Sleep(1);
                }
            });

            Task.WaitAll(task1, task2);
        }
    }
}