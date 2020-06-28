using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     Extension methods for <see cref="ICacheManager" />.
    ///     <para><see cref="ICacheManager" /> 的扩展方法</para>
    /// </summary>
    public static class CacheManagerExtensions
    {
        /// <summary>
        ///     返回指定名称的缓存
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ITypedCache<TKey, TValue> GetCache<TKey, TValue>(this ICacheManager cacheManager, string name)
        {
            return cacheManager.GetCache(name).AsTyped<TKey, TValue>();
        }
    }
}
