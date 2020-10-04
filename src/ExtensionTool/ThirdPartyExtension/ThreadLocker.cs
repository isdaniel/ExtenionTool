using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Castle.DynamicProxy;

namespace ThirdPartyExtension
{

    public class ThreadLocker
    {
       private ConcurrentDictionary<string, object> _processFlags = new ConcurrentDictionary<string, object>();

       public object GetProcessFlag(string key)
       {
           return _processFlags.GetOrAdd(key, new object());
       }
    }

    public class LockByAttribute : Attribute
    {
        public string Key { get; set; }
    }

    public class LockInterceptor : IInterceptor
    {
        private ThreadLocker _threadLocker;
        private ISysLog _log;
        public LockInterceptor(ThreadLocker threadLocker,ISysLog log)
        {
            this._threadLocker = threadLocker;
            _log = log;
        }

        public void Intercept(IInvocation invocation)
        {
            var lockAttribute = invocation.Method.GetCustomAttribute(typeof(LockByAttribute), true) as LockByAttribute;
            if (lockAttribute!=null)
            {
                var lockObj = _threadLocker.GetProcessFlag(lockAttribute.Key);

                try
                {

                    lock (lockObj)
                    {
                        _log.Info($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff} processing {invocation.Method.Name}");
                        invocation.Proceed();
                    }
                }
                catch (Exception e)
                {
                    _log.Exception("Something wrong!",e);
                    throw e;
                }

            }
            else
            {
                invocation.Proceed();
            }
           
        }
    }

}
