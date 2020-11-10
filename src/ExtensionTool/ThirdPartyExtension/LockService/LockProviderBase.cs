using System.Threading;

namespace ThirdPartyExtension.LockService
{
    public abstract class LockProviderBase
    {
        protected ReaderWriterLockSlim LockObj { get; private set; }

        protected LockProviderBase(ReaderWriterLockSlim lockObj)
        {
            LockObj = lockObj;
        }

        public abstract void AddLock();

        public abstract void ReleaseLock();
    }
}