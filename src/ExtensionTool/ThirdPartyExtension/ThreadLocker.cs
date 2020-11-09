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
        
        private ConcurrentDictionary<string, ReaderWriterLockSlim> _processFlagMapper;

        private ThreadLocker()
        {
            _processFlagMapper = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
        }

        public ReaderWriterLockSlim GetProcessFlag(string key)
        {
            return  _processFlagMapper.GetOrAdd(key,new ReaderWriterLockSlim());
        }
    }
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LockByAttribute : Attribute
    {
        public string Key { get; set; }

        public LockMode Mode { get; set; } = LockMode.XLock;

        public int Order{ get; set; }
    }

    public enum LockMode
    {
        SharedLock,
        XLock
    }

    internal class LockFactory {
        private ReaderWriterLockSlim LockObj { get; set; }

        internal LockFactory(ReaderWriterLockSlim _lockObj)
        {
            LockObj = _lockObj;
        }

        public LockProviderBase GetLockProvider (LockMode lockMode)
        {
            if (lockMode == LockMode.SharedLock)
            {
                return new SharedLockProvider(LockObj);
            }
            else
            {
                return new XLockProvider(LockObj);
            }
        }
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
            var methodName = invocation.Method.Name;
            var lockAttributes = invocation.Method.GetCustomAttributes(typeof(LockByAttribute), true) as LockByAttribute[];


            if (IsMarkLockLockAttribute(lockAttributes))
            {
                var lockProviders = GetLockProviders(lockAttributes);
                try
                {
                    foreach (var lockProvider in lockProviders)
                    {
                        lockProvider.AddLock();
                    }
                    invocation.Proceed();
                }
                catch (Exception e)
                {
                    _log.Exception("Something wrong!", e);
                    throw e;
                }
                finally {
                    foreach (var lockProvider in lockProviders)
                    {
                        lockProvider.ReleaseLock();
                    }
                    _log.Info($"{DateTime.Now:HH:mm:ss fff} {methodName} Release Lock");
                }
            }
            else
            {
                invocation.Proceed();
            }
           
        }

        
        private bool IsValidateLockAttribute(LockByAttribute lockAttribute)
        {
            return lockAttribute != null && !string.IsNullOrEmpty(lockAttribute.Key);
        }

        private bool IsMarkLockLockAttribute (LockByAttribute[] attrs)
        {
            return attrs != null && attrs.Any();
        }

        private List<LockProviderBase> GetLockProviders(LockByAttribute[] lockAttributes)
        {
            var lockProviders = lockAttributes.Where(IsValidateLockAttribute)
                .Select(lockAttribute =>
                {
                    var lockObj = ThreadLocker.Instance.GetProcessFlag(lockAttribute.Key);
                    LockProviderBase lockProvider = new LockFactory(lockObj).GetLockProvider(lockAttribute.Mode);
                    return lockProvider;
                }).ToList();

            return lockProviders;
        }
    }

}
