using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     通过Type的方式来使用的缓存接口定义
    /// </summary>
    /// <typeparam name="TKey">
    ///     用于作为key的类型
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     缓存的数据
    /// </typeparam>
    public interface ITypedCache<TKey, TValue> : IDisposable
    {
        /// <summary>
        ///     缓存中的唯一标识名字
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     默认过期时间
        /// </summary>
        TimeSpan DefaultExpireTime { get; set; }

        /// <summary>
        ///     获取其内部缓存实例
        /// </summary>
        ICache InternalCache { get; }

        /// <summary>
        ///     默认缓存过期类型.
        /// </summary>
        CacheExpireType DefaultExpireType { get; set; }

        /// <summary>
        ///     通过key从缓存中获取数据
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不含此key则通过factory来创建一个实例置入缓存.
        /// </param>
        /// <returns>
        ///     缓存中的数据.
        /// </returns>
        TValue Get(TKey key, Func<TKey, TValue> factory);

        /// <summary>
        ///     通过key从缓存中获取数据
        /// </summary>
        /// <param name="keys">Key</param>
        /// <param name="factory">
        ///     如果缓存中不含此key则通过factory来创建一个实例置入缓存.
        /// </param>
        /// <returns>
        ///     缓存中的数据.
        /// </returns>
        TValue[] Get(TKey[] keys, Func<TKey, TValue> factory);

        /// <summary>
        ///     通过key从缓存中获取数据.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不含此key则通过factory来创建一个实例置入缓存.
        /// </param>
        /// <returns>
        ///     缓存中的数据.
        /// </returns>
        Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory);

        /// <summary>
        ///     通过key从缓存中获取数据.
        /// </summary>
        /// <param name="keys">Key</param>
        /// <param name="factory">
        ///     如果缓存中不含此key则通过factory来创建一个实例置入缓存.
        /// </param>
        /// <returns>
        ///     缓存中的数据.
        /// </returns>
        Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory);

        /// <summary>
        ///     通过key从缓存中获取数据.如果key不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存中的数据或null.
        /// </returns>
        TValue GetOrDefault(TKey key);

        /// <summary>
        ///     通过key从缓存中获取数据.如果key不存在则返回null.
        /// </summary>
        /// <param name="keys">Key</param>
        /// <returns>
        ///     缓存中的数据或null.
        /// </returns>
        TValue[] GetOrDefault(TKey[] keys);

        /// <summary>
        ///     通过key从缓存中获取数据.如果key不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存中的数据或null.
        /// </returns>
        Task<TValue> GetOrDefaultAsync(TKey key);

        /// <summary>
        ///     通过key从缓存中获取数据.如果key不存在则返回null.
        /// </summary>
        /// <param name="keys">Key</param>
        /// <returns>
        ///     缓存中的数据或null.
        /// </returns>
        Task<TValue[]> GetOrDefaultAsync(TKey[] keys);

        /// <summary>
        ///     通过key将数据存入缓存.如果该key已存在原数据将会被覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        void Set(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     通过key将数据存入缓存.如果该key已存在原数据将会被覆盖.
        /// </summary>
        /// <param name="pairs">Key</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     通过key将数据存入缓存.如果该key已存在原数据将会被覆盖
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        Task SetAsync(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     通过key将数据存入缓存.如果该key已存在原数据将会被覆盖.
        /// </summary>
        /// <param name="pairs">Key</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">过期时间类型</param>
        Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     通过key删除缓存项(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(TKey key);

        /// <summary>
        ///     通过key删除缓存项(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="keys">Key</param>
        void Remove(TKey[] keys);

        /// <summary>
        ///     通过key删除缓存项(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(TKey key);

        /// <summary>
        ///     通过key删除缓存项(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="keys">Key</param>
        Task RemoveAsync(TKey[] keys);


        /// <summary>
        ///     清空缓存
        /// </summary>
        void Clear();

        /// <summary>
        ///     清空缓存
        /// </summary>
        Task ClearAsync();
    }
}
