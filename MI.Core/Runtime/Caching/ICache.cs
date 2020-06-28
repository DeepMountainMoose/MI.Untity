using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     定义一个可以通过key来存储和读取数据的缓存.
    /// </summary>
    public interface ICache : IDisposable
    {
        /// <summary>
        ///     缓存的名称,该名称在当前缓存中必须唯一.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     默认的过期时间.
        /// </summary>
        TimeSpan DefaultExpireTime { get; set; }

        /// <summary>
        ///     默认缓存过期类型.
        /// </summary>
        CacheExpireType DefaultExpireType { get; set; }

        #region [ Get ]

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        /// <returns>
        ///     从缓存中返回的对象
        /// </returns>
        object Get(string key, Func<string, object> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        /// <returns>
        ///     从缓存中返回的对象
        /// </returns>
        TValue Get<TKey, TValue>(TKey key, Func<TKey, TValue> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">如果缓存中不存在的话,将返回由该委托创建的对象</param>
        /// <returns>从缓存中返回的对象</returns>
        object[] Get(string[] keys, Func<string, object> factory);

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        /// <returns>
        ///     从缓存中返回的对象
        /// </returns>
        Task<object> GetAsync(string key, Func<string, Task<object>> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">
        ///     如果缓存中不存在的话,将返回由该委托创建的对象
        /// </param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        /// <returns>
        ///     从缓存中返回的对象
        /// </returns>
        Task<TValue> GetAsync<TKey, TValue>(TKey key, Func<TKey, Task<TValue>> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     从缓存中获取一个对象.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">如果缓存中不存在的话,将返回由该委托创建的对象</param>
        /// <returns>从缓存中返回的对象</returns>
        Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory);

        #endregion

        #region [ GetOrDefault ]

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        object GetOrDefault(string key);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        TValue GetOrDefault<TKey, TValue>(TKey key);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>缓存的对象,如果该对象不存在于缓存中则返回默认值</returns>
        object[] GetOrDefault(string[] keys);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>缓存的对象,如果该对象不存在于缓存中则返回默认值</returns>
        TValue[] GetOrDefault<TKey, TValue>(TKey[] keys);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        Task<TValue> GetOrDefaultAsync<TKey, TValue>(TKey key);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        Task<object[]> GetOrDefaultAsync(string[] keys);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回null.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        Task<TValue[]> GetOrDefaultAsync<TKey, TValue>(TKey[] keys);

        #endregion

        #region [ Set ]

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        void Set(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        void Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        void Set(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        Task SetAsync(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期时间类型
        /// </param>
        Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null,
            CacheExpireType? expireType = null);

        #endregion

        #region [ Remove ]

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        ///     通过key删除缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="keys">Keys</param>
        void Remove(string[] keys);

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(string key);

        /// <summary>
        ///     通过key删除缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="keys">Keys</param>
        Task RemoveAsync(string[] keys);

        #endregion

        #region [ Clear ]

        /// <summary>
        ///     清空缓存中所有对象.
        /// </summary>
        void Clear();

        /// <summary>
        ///     清空缓存中所有对象.
        /// </summary>
        Task ClearAsync();

        #endregion
    }
}
