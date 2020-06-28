using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     Extension methods for <see cref="ICache" />.
    /// </summary>
    public static class CacheExtensions
    {

        public static TValue[] Get<TKey, TValue>(this ICache cache, TKey[] keys, Func<TKey, TValue> factory)
        {
            return keys.Select(key => (TValue)cache.Get(key.ToString(), (k) => (object)factory(key))).ToArray();
        }

        public static TValue[] Get<TKey, TValue>(this ICache cache, TKey[] keys, Func<TValue> factory)
        {
            return keys.Select(key => cache.Get(key, (k) => factory())).ToArray();
        }

        public static async Task<TValue[]> GetAsync<TKey, TValue>(this ICache cache, TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            var tasks = keys.Select(key =>
            {
                return cache.GetAsync(key.ToString(), async (keyAsString) =>
                {
                    var v = await factory(key);
                    return (object)v;
                });
            });
            var values = await Task.WhenAll(tasks);
            return values.Select(value => (TValue)value).ToArray();
        }

        public static Task<TValue[]> GetAsync<TKey, TValue>(this ICache cache, TKey[] keys, Func<Task<TValue>> factory)
        {
            var tasks = keys.Select(key => cache.GetAsync(key, (k) => factory()));
            return Task.WhenAll(tasks);
        }


        /// <summary>
        ///     Convert <see cref="ICache" /> to <see cref="ITypedCache{TKey,TValue}" />
        ///     <para>将<see cref="ICache" /> 转换为 <see cref="ITypedCache{TKey,TValue}" /></para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static ITypedCache<TKey, TValue> AsTyped<TKey, TValue>(this ICache cache)
        {
            return new TypedCacheWrapper<TKey, TValue>(cache);
        }





    }
}
