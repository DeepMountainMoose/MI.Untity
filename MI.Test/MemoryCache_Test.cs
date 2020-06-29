using MI.Core.Configuration.Startup;
using MI.Core.Dependency;
using MI.Core.Runtime.Caching;
using MI.Core.Runtime.Caching.Configuration;
using MI.Core.Runtime.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MI.Test
{
    public class MemoryCache_Test
    {
        private readonly IocManager _iocManager;

        public MemoryCache_Test()
        {
            _iocManager = new IocManager();
        }

        /// <summary>
        /// 返回Cache实例
        /// </summary>
        /// <returns></returns>
        private ICache GetCache()
        {
            var cacheManager = new MemoryCacheManager(_iocManager, new CachingConfiguration(new StartupConfiguration(_iocManager)));
            return cacheManager.GetCache("defaultcache");
        }

        private class CacheItem
        {
            public DateTime DateTime { get; set; }
        }

        [Fact]
        public void Cache_SetAndGet()
        {
            var cache = GetCache();
             var cacheItem = new CacheItem { DateTime = DateTime.Now };
            cache.Set("test", cacheItem);
            var getFromCacheItem = cache.GetOrDefault("test");
            Assert.Same(cacheItem, getFromCacheItem);
        }
    }
}
