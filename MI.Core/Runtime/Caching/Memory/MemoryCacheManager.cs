using MI.Core.Dependency;
using MI.Core.Extensions;
using MI.Core.Runtime.Caching.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MI.Core.Runtime.Caching.Memory
{
    /// <summary>
    /// Implements <see cref="ICacheManager"/> to work with <see cref="MemoryCache"/>.
    /// </summary>
    [DebuggerDisplay("MemoryCache Cache Manager. Cache Count {Caches.Count}")]
    public class MemoryCacheManager : CacheManagerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MemoryCacheManager(IIocManager iocManager, ICachingConfiguration configuration)
            : base(iocManager, configuration)
        {
            IocManager.RegisterTypeIfNot<MemoryCache>(DependencyLifeStyle.Transient);
        }

        /// <summary>
        ///     Used to create actual cache implementation.
        /// </summary>
        /// <param name="name">Name of the cache</param>
        /// <returns>Cache object</returns>
        protected override ICache CreateCacheImplementation(string name)
        {
            return IocManager.Resolve<MemoryCache>(new { name });
        }
    }
}
