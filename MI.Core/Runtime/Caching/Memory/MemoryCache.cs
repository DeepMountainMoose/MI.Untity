using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime.Caching.Memory
{
    /// <summary>
    ///     内存缓存的<see cref="ICache"/>实现.
    /// </summary>
    public class MemoryCache : CacheBase
    {
        private Microsoft.Extensions.Caching.Memory.MemoryCache _memoryCache;
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Unique name of the cache</param>
        public MemoryCache(string name)
            : base(name)
        {
            _memoryCache =
                new Microsoft.Extensions.Caching.Memory.MemoryCache(
                    new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public override object GetOrDefault(string key)
        {
            return _memoryCache.Get(key);
        }

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public override TValue GetOrDefault<TKey, TValue>(TKey key)
        {
            return _memoryCache.Get<TValue>(key.ToString());
        }

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期类型
        /// </param>
        public override void Set(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            if (value == null)
            {
                throw new CoreException("Can not insert null values to the cache!");
            }
            var eType = expireType ?? DefaultExpireType;
            switch (eType)
            {
                case CacheExpireType.Absolute:
                    _memoryCache.Set(key, value, expireTime.HasValue
                        ? DateTimeOffset.Now.Add(expireTime.Value)
                        : DateTimeOffset.Now.Add(DefaultExpireTime));
                    break;
                case CacheExpireType.Slide:
                    _memoryCache.Set(key, value, expireTime ?? DefaultExpireTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作).
        /// </summary>
        /// <param name="key">Key</param>
        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        ///     清空缓存中所有对象.
        /// </summary>
        public override void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache =
                new Microsoft.Extensions.Caching.Memory.MemoryCache(
                    new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
        }

        /// <summary>
        ///     释放当前缓存实例.
        /// </summary>
        public override void Dispose()
        {
            _memoryCache.Dispose();
            base.Dispose();
        }
    }
}
