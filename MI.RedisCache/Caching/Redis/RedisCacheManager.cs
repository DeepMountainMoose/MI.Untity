using MI.Core.Dependency;
using MI.Core.Extensions;
using MI.Core.Runtime.Caching;
using MI.Core.Runtime.Caching.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> using Redis
    /// </summary>
    public class RedisCacheManager : CacheManagerBase
    {
        public RedisCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterTypeIfNot<RedisCache>(DependencyLifeStyle.Transient);
        }
        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<RedisCache>(new { name });
        }
    }
}
