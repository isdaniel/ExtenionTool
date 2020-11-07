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

    internal class ThreadLocker : Singleton<ThreadLocker>
    {
        
        private ConcurrentDictionary<string, object> _processFlagMapper;

        private ThreadLocker()
        {
            _processFlagMapper = new ConcurrentDictionary<string, object>();
        }

        public object GetProcessFlag(string key)
        {
            return  _processFlagMapper.GetOrAdd(key,new object());
        }
    }
    

    public class LockByAttribute : Attribute
    {
        public string Key { get; set; }
    }

    public class LockInterceptor : IInterceptor
    {
        private ISysLog _log;
        public LockInterceptor(ISysLog log)
        {
            _log = log;
        }

        public void Intercept(IInvocation invocation)
        {
            var lockAttribute = invocation.Method.GetCustomAttribute(typeof(LockByAttribute), true) as LockByAttribute;
            if (lockAttribute!=null)
            {
                var lockObj = ThreadLocker.Instance.GetProcessFlag(lockAttribute.Key);

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
