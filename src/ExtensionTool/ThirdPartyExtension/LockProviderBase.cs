using System.Threading;

namespace ThirdPartyExtension
{
    public abstract class LockProviderBase
    {
        protected ReaderWriterLockSlim LockObj { get; private set; }
        public LockProviderBase(ReaderWriterLockSlim _lockObj)
        {
            LockObj = _lockObj;
        }

        public abstract void AddLock();

        public abstract void ReleaseLock();
    }

    public class SharedLockProvider : LockProviderBase
    {
        public SharedLockProvider(ReaderWriterLockSlim _lockObj) : base(_lockObj)
        {
        }

        public override void AddLock()
        {
            LockObj.EnterReadLock();
        }

        public override void ReleaseLock()
        {
            LockObj.ExitReadLock();
        }
    }

    public class XLockProvider : LockProviderBase
    {
        public XLockProvider(ReaderWriterLockSlim _lockObj) : base(_lockObj)
        {
        }

        public override void AddLock()
        {
            LockObj.EnterWriteLock();
        }

        public override void ReleaseLock()
        {
            LockObj.ExitWriteLock();
        }
    }
}