using System.Threading;

namespace ThirdPartyExtension.LockService
{
    public class XLockProvider : LockProviderBase
    {
        public XLockProvider(ReaderWriterLockSlim lockObj) : base(lockObj)
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