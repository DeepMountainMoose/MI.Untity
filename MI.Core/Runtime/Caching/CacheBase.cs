using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     缓存基类
    /// </summary>
    public abstract class CacheBase : ICache
    {
        public ILogger Logger { get; set; }

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name)
        {
            Name = name;
            DefaultExpireTime = TimeSpan.FromHours(1);
            DefaultExpireType = CacheExpireType.Absolute;

            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///     缓存的名称,该名称在当前缓存中必须唯一
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     默认绝对过期时间
        /// </summary>
        public TimeSpan DefaultExpireTime { get; set; }

        /// <summary>
        ///     默认缓存过期类型
        /// </summary>
        public CacheExpireType DefaultExpireType { get; set; }

        /// <summary>
        ///     释放该缓存对象
        /// </summary>
        public virtual void Dispose() { }

        #region [ Get ] 

        /// <summary>
        ///     从缓存中获取一个对象
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
        public virtual object Get(string key, Func<string, object> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            object item = null;

            try
            {
                item = GetOrDefault(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
                return factory(key);
            }

            if (item == null)
            {
                item = factory(key);

                if (item == null)
                {
                    return null;
                }

                try
                {
                    Set(key, item, expireTime, expireType);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return item;
        }

        /// <summary>
        ///     从缓存中获取一个对象
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
        public virtual TValue Get<TKey, TValue>(TKey key, Func<TKey, TValue> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            TValue item = default;

            try
            {
                item = GetOrDefault<TKey, TValue>(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
                return factory(key);
            }

            if (item == null || item.Equals(default(TValue)))
            {
                item = factory(key);

                if (item == null)
                {
                    return default;
                }

                try
                {
                    Set(key, item, expireTime, expireType);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return item;
        }

        public virtual object[] Get(string[] keys, Func<string, object> factory)
        {
            object[] items = null;

            try
            {
                items = GetOrDefault(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (items == null)
            {
                items = new object[keys.Length];
            }

            var fetched = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < items.Length; i++)
            {
                var key = keys[i];
                var value = items[i] ?? factory(key);

                if (value != null)
                {
                    fetched.Add(new KeyValuePair<string, object>(key, value));
                }
            }

            if (fetched.Any())
            {
                try
                {
                    Set(fetched.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return items;
        }

        /// <summary>
        ///     从缓存中获取一个对象
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
        public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            object item = null;

            try
            {
                item = await GetOrDefaultAsync(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
                return await factory(key);
            }

            if (item == null)
            {
                item = await factory(key);

                if (item == null)
                {
                    return null;
                }

                try
                {
                    await SetAsync(key, item, expireTime, expireType);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return item;
        }

        /// <summary>
        ///     从缓存中获取一个对象
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
        public virtual async Task<TValue> GetAsync<TKey, TValue>(TKey key, Func<TKey, Task<TValue>> factory, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            TValue item = default;

            try
            {
                item = await GetOrDefaultAsync<TKey, TValue>(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
                return await factory(key);
            }

            if (item == null || item.Equals(default(TValue)))
            {
                item = await factory(key);

                if (item == null)
                {
                    return default;
                }

                try
                {
                    await SetAsync(key, item, expireTime, expireType);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return item;
        }

        public virtual async Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory)
        {
            object[] items = null;

            try
            {
                items = await GetOrDefaultAsync(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (items == null)
            {
                items = new object[keys.Length];
            }

            var fetched = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < items.Length; i++)
            {
                var key = keys[i];
                var value = items[i] ?? factory(key);

                if (value != null)
                {
                    fetched.Add(new KeyValuePair<string, object>(key, value));
                }
            }

            if (fetched.Any())
            {
                try
                {
                    await SetAsync(fetched.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }
            }

            return items;
        }
        #endregion

        #region [ GetOrDefault ]

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public abstract object GetOrDefault(string key);

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public abstract TValue GetOrDefault<TKey, TValue>(TKey key);

        public virtual object[] GetOrDefault(string[] keys)
        {
            return keys.Select(GetOrDefault).ToArray();
        }

        public virtual TValue[] GetOrDefault<TKey, TValue>(TKey[] keys)
        {
            var values = keys.Select(key =>
            {
                var value = GetOrDefault(key.ToString());
                if (value == null)
                {
                    return default;
                }
                return (TValue)value;
            });

            return values.ToArray();
        }

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public virtual Task<object> GetOrDefaultAsync(string key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        /// <summary>
        ///     从缓存中获取一个对象, 如果该对象不存在则返回默认值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>
        ///     缓存的对象,如果该对象不存在于缓存中则返回默认值
        /// </returns>
        public virtual Task<TValue> GetOrDefaultAsync<TKey, TValue>(TKey key)
        {
            return Task.FromResult(GetOrDefault<TKey, TValue>(key));
        }

        public virtual Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            return Task.FromResult(GetOrDefault(keys));
        }

        public virtual async Task<TValue[]> GetOrDefaultAsync<TKey, TValue>(TKey[] keys)
        {
            var tasks = keys.Select(async (key) =>
            {
                var value = await GetOrDefaultAsync<TKey, TValue>(key);
                if (value == null)
                {
                    return default;
                }
                return value;
            });

            return await Task.WhenAll(tasks);
        }

        #endregion

        #region [ Set ]

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期类型
        /// </param>
        public abstract void Set(string key, object value, TimeSpan? expireTime = null,
            CacheExpireType? expireType = null);

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期类型
        /// </param>
        public virtual void Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null,
            CacheExpireType? expireType = null)
        {
            Set(key.ToString(), (object)value, expireTime, expireType);
        }

        public virtual void Set(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null,
            CacheExpireType? expireType = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, expireTime, expireType);
            }
        }

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期类型
        /// </param>
        public virtual Task SetAsync(string key, object value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            Set(key, value, expireTime, expireType);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     存入一个对象到缓存,如果该对象已经存在则覆盖
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">
        ///     过期时间
        /// </param>
        /// <param name="expireType">
        ///     过期类型
        /// </param>
        public virtual Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            Set(key, value, expireTime, expireType);
            return Task.CompletedTask;
        }

        public virtual Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? expireTime = null, CacheExpireType? expireType = null)
        {
            return Task.WhenAll(pairs.Select(p => SetAsync(p.Key, p.Value, expireTime, expireType)));
        }

        #endregion

        #region [ Remove ]

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        public abstract void Remove(string key);

        /// <summary>
        ///     通过key删除缓存对象
        /// </summary>
        /// <param name="keys"></param>
        public virtual void Remove(string[] keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        /// <summary>
        ///     通过key删除一个缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="key">Key</param>
        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     通过key删除缓存对象(如果key不存在则不执行任何操作)
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual Task RemoveAsync(string[] keys)
        {
            return Task.WhenAll(keys.Select(RemoveAsync));
        }

        #endregion

        #region [ Clear ]

        /// <summary>
        ///     Clears all items in this cache.
        ///     <para>清空缓存中所有对象.</para>
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     清空缓存中所有对象
        /// </summary>
        public virtual Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        #endregion
    }
}
