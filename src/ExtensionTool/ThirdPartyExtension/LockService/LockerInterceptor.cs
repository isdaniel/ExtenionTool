using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using ThirdPartyExtension.LockService;

namespace ThirdPartyExtension
{
    public class LockerInterceptor : IInterceptor
    {
        private readonly ISysLog _log;
        public LockerInterceptor(ISysLog log)
        {
            _log = log;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            var lockAttributes = invocation.Method.GetCustomAttributes(typeof(LockAttribute), true) as LockAttribute[];


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

        
        private bool IsValidateLockAttribute(LockAttribute lockAttribute)
        {
            return lockAttribute != null && !string.IsNullOrEmpty(lockAttribute.Key);
        }

        private bool IsMarkLockLockAttribute (LockAttribute[] attrs)
        {
            return attrs != null && attrs.Any();
        }

        private List<LockProviderBase> GetLockProviders(LockAttribute[] lockAttributes)
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