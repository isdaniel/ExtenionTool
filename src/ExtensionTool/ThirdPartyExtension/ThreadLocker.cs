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
    public class ProcessFlag 
    {
        private int usingResource;
        private const int STOPPING = 0;
        private const int PROCESSING = 1;
        public bool IsProcessing => Thread.VolatileRead(ref usingResource) == PROCESSING;

        public void StopProcess()
        {
            Interlocked.Exchange(ref usingResource, STOPPING);
        }

        public void InProcess()
        {
            Interlocked.Exchange(ref usingResource, PROCESSING);
        }
    }

    public class ThreadLocker
    {
       private ConcurrentDictionary<string, ProcessFlag> _processFlags = new ConcurrentDictionary<string, ProcessFlag>();

       public ProcessFlag GetProcessFlag(string key)
       {
           return _processFlags.GetOrAdd(key, new ProcessFlag());
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

            if (invocation.Method.GetCustomAttribute(typeof(LockByAttribute), true) is LockByAttribute lockAttribute)
            {
                var processFlag = _threadLocker.GetProcessFlag(lockAttribute.Key);

                try
                {
                    string status = processFlag.IsProcessing ? "waiting for another process": "processing";
                    _log.Info($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff}  {invocation.Method.Name} : {status}");
                    while (!processFlag.IsProcessing)
                    {
                        processFlag.InProcess();
                        _log.Info($"{DateTime.Now:MM/dd/yyyy HH:mm:ss fff} processing {invocation.Method.Name}");
                        invocation.Proceed();
                    }

                    processFlag.StopProcess();
                }
                catch (Exception e)
                {
                    processFlag.StopProcess();
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
