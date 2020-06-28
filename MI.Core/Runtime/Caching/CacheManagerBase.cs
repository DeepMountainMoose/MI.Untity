using MI.Core.Dependency;
using MI.Core.Runtime.Caching.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     Base class for cache managers.
    ///     <para>缓存管理器的基类</para>
    /// </summary>
    public abstract class CacheManagerBase : ICacheManager, ISingletonDependency
    {
        /// <summary>
        ///     当前已实例化的缓存
        /// </summary>
        protected readonly ConcurrentDictionary<string, ICache> Caches;

        /// <summary>
        ///     缓存配置
        /// </summary>
        protected readonly ICachingConfiguration Configuration;

        /// <summary>
        ///     用于当前缓存管理器的<see cref="IIocManager"/>实例
        /// </summary>
        protected readonly IIocManager IocManager;

        /// <summary>
        ///     构造函数.
        /// </summary>
        /// <param name="iocManager"></param>
        /// <param name="configuration"></param>
        protected CacheManagerBase(IIocManager iocManager, ICachingConfiguration configuration)
        {
            IocManager = iocManager;
            Configuration = configuration;
            Caches = new ConcurrentDictionary<string, ICache>();
        }

        /// <summary>
        ///     返回所有已实例化的缓存
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ICache> GetAllCaches()
        {
            return Caches.Values.ToImmutableList();
        }

        /// <summary>
        ///     返回指定名称的缓存实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ICache GetCache(string name)
        {
            Check.NotNull(name, nameof(name));

            return Caches.GetOrAdd(name, cacheName =>
            {
                var cache = CreateCacheImplementation(cacheName);

                var configurators =
                    Configuration.Configurators.Where(c => c.CacheName == null || c.CacheName == cacheName);

                foreach (var configurator in configurators)
                {
                    configurator.InitAction?.Invoke(cache);
                }

                return cache;
            });
        }

        /// <summary>
        ///     释放当前的缓存管理器
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var cache in Caches)
            {
                IocManager.Release(cache.Value);
            }

            Caches.Clear();
        }

        /// <summary>
        ///     Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected abstract ICache CreateCacheImplementation(string name);
    }
}
