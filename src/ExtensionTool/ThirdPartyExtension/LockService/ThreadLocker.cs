using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace ThirdPartyExtension
{

    internal class ThreadLocker : Singleton<ThreadLocker>
    {
        
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _processFlagMapper;

        private ThreadLocker()
        {
            _processFlagMapper = new ConcurrentDictionary<string, ReaderWriterLockSlim>();
        }

        public ReaderWriterLockSlim GetProcessFlag(string key)
        {
            return  _processFlagMapper.GetOrAdd(key,new ReaderWriterLockSlim());
        }
    }
}
