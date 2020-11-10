using System.Threading;

namespace ThirdPartyExtension.LockService
{
    public class SharedLockProvider : LockProviderBase
    {
        public SharedLockProvider(ReaderWriterLockSlim lockObj) : base(lockObj)
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
}