using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace ExtensionTool
{
    public class WebDefaultCache : ICache
    {
        readonly Cache cacheContainer = HttpRuntime.Cache;
        public object Get(string key)
        {
            return cacheContainer.Get(key);
        }

        public void Set(string key, object obj)
        {
            cacheContainer.Insert(key, obj);
        }
    }
    public interface ICache{
        void Set(string key, object obj);

        object Get(string key);
    }

    public static class InfrastructureExtension
    {
        public static TObj GetOrSetCache<TObj>(this Func<TObj> selector, string key) where TObj : class {
            return GetOrSetCache(selector, key,10);
        }

        public static TObj GetOrSetCache<TObj>(this Func<TObj> selector, string key, int cacheTime) where TObj : class
        {
            return GetOrSetCache(selector, key, cacheTime, new WebDefaultCache());
        }

        public static TObj GetOrSetCache<TObj>(this Func<TObj> selector, string key, int cacheTime, ICache cacheContainer) where TObj : class
        {
            //get cache Object
            var obj = cacheContainer.Get(key) as TObj;

            //if there isn't cache object add this object to cache
            if (obj == null)
            {
                obj = selector();
                cacheContainer.Set(key, obj);
            }

            return obj;
        }
    }
}
