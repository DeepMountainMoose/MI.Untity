using System;
using System.Threading.Tasks;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ITypedCache{TKey,TValue}"/>.
    /// <para><see cref="ITypedCache{TKey,TValue}"/> 的扩展方法.</para>
    /// </summary>
    public static class TypedCacheExtensions
    {
        /// <summary>
        ///     Gets an item from the cache.
        ///     <para>从缓存中获取一个对象.</para>
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists
        ///     <para>如果缓存中不存在的话,将返回由该委托创建的对象</para>
        /// </param>
        /// <returns>Cached item
        ///     <para>从缓存中返回的对象</para>
        /// </returns>
        public static TValue Get<TKey, TValue>(this ITypedCache<TKey, TValue> cache, TKey key, Func<TValue> factory)
        {
            return cache.Get(key, k => factory());
        }

        /// <summary>
        ///     Gets an item from the cache.
        ///     <para>从缓存中获取一个对象.</para>
        /// </summary>
        /// <param name="cache">Cache instance</param>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists
        ///     <para>如果缓存中不存在的话,将返回由该委托创建的对象</para>
        /// </param>
        /// <returns>Cached item
        ///     <para>从缓存中返回的对象</para>
        /// </returns>
        public static Task<TValue> GetAsync<TKey, TValue>(this ITypedCache<TKey, TValue> cache, TKey key, Func<Task<TValue>> factory)
        {
            return cache.GetAsync(key, k => factory());
        }
    }
}
