using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     Implements <see cref="ITypedCache{TKey,TValue}" /> to wrap a <see cref="ICache" />.
    ///     <para>使用强类型包装 <see cref="ICache" />的 <see cref="ITypedCache{TKey,TValue}" /> 接口的实现.</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TypedCacheWrapper<TKey, TValue> : ITypedCache<TKey, TValue>
    {
        /// <summary>
        ///     Creates a new <see cref="TypedCacheWrapper{TKey,TValue}" /> object.
        /// </summary>
        /// <param name="internalCache">The actual internal cache</param>
        public TypedCacheWrapper(ICache internalCache)
        {
            InternalCache = internalCache;
        }

        /// <summary>
        ///     Get name of the cache.
        ///     <para>获取缓存的名称.</para>
        /// </summary>
        public string Name => InternalCache.Name;

        /// <summary>
        ///     Default absolute expire time of cache items.
        ///     <para>默认的绝对过期时间.</para>
        /// </summary>
        public TimeSpan DefaultExpireTime
        {
            get { return InternalCache.DefaultExpireTime; }
            set { InternalCache.DefaultExpireTime = value; }
        }

        /// <summary>
        ///     Default expire type.
        ///     <para>默认缓存过期类型</para>
        /// </summary>
        public CacheExpireType DefaultExpireType
        {
            get { return InternalCache.DefaultExpireType; }
            set { InternalCache.DefaultExpireType = value; }
        }

        /// <summary>
        ///     内部引用的<see cref="ICache"/>
        /// </summary>
        public ICache InternalCache { get; }

        /// <summary>
        ///     释放当前缓存
        /// </summary>
        public void Dispose()
        {
            InternalCache.Dispose();
        }

        /// <summary>
        ///     清空当前缓存的数据
        /// </summary>
        public void Clear()
        {
            InternalCache.Clear();
        }

        /// <summary>
        ///     清空当前缓存的数据
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            return InternalCache.ClearAsync();
        }

        /// <summary>
        ///    从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <returns>从缓存中返回的对象
        /// </returns>
        public TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            return InternalCache.Get(key, factory);
        }

        public TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
        {
            return InternalCache.Get(keys, factory);
        }

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <returns>从缓存中返回的对象
        /// </returns>
        public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return InternalCache.GetAsync(key, factory);
        }

        public Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            return InternalCache.GetAsync(keys, factory);
        }

        /// <summary>
        ///    从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public TValue GetOrDefault(TKey key)
        {
            return InternalCache.GetOrDefault<TKey, TValue>(key);
        }

        public TValue[] GetOrDefault(TKey[] keys)
        {
            return InternalCache.GetOrDefault<TKey, TValue>(keys);
        }

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return InternalCache.GetOrDefaultAsync<TKey, TValue>(key);
        }

        public Task<TValue[]> GetOrDefaultAsync(TKey[] keys)
        {
            return InternalCache.GetOrDefaultAsync<TKey, TValue>(keys);
        }

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">Sliding expire time
        ///     <para></para>
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        public void Set(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            InternalCache.Set(key, value, expireTime, expireType);
        }

        public void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
            InternalCache.Set(stringPairs.ToArray(), expireTime, expireType);
        }

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">Sliding expire time
        ///     <para>过期时间</para>
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        public Task SetAsync(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            return InternalCache.SetAsync(key, value, expireTime, expireType);
        }

        public Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
            return InternalCache.SetAsync(stringPairs.ToArray(), expireTime, expireType);
        }

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(TKey key)
        {
            InternalCache.Remove(key.ToString());
        }

        public void Remove(TKey[] keys)
        {
            InternalCache.Remove(keys.Select(key => key.ToString()).ToArray());
        }


        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        public Task RemoveAsync(TKey key)
        {
            return InternalCache.RemoveAsync(key.ToString());
        }

        public Task RemoveAsync(TKey[] keys)
        {
            return InternalCache.RemoveAsync(keys.Select(key => key.ToString()).ToArray());
        }
    }
}
