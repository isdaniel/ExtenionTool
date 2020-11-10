using System.Threading;
using ThirdPartyExtension.LockService;

namespace ThirdPartyExtension
{
    internal class LockFactory {
        private ReaderWriterLockSlim LockObj { get; set; }

        internal LockFactory(ReaderWriterLockSlim lockObj)
        {
            LockObj = lockObj;
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
}