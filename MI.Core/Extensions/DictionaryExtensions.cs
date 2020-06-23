using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     <see cref="IDictionary{TKey,TValue}" /> 的扩展方法.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     尝试从Dictionary读取数据.如果数据不存在则返回value的默认值
        /// </summary>
        /// <typeparam name="T">
        ///     value的类型
        /// </typeparam>
        /// <param name="dictionary">
        ///     给定的Dictionary
        /// </param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>
        ///     如果key在dictionary是存在的则返回true
        /// </returns>
        internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object valueObj;
            if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
            {
                value = (T)valueObj;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        ///     尝试性从Dictionary读取数据.如果数据不存在则返回value的默认值
        /// </summary>
        /// <param name="dictionary">
        ///     给定的Dictionary
        /// </param>
        /// <param name="key">Key</param>
        /// <typeparam name="TKey">
        ///     key的类型
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     value的类型
        /// </typeparam>
        /// <returns>
        ///     如果key存在则返回查找后的结果,否则返回value对应的默认值.
        /// </returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            return dictionary.TryGetValue(key, out obj) ? obj : default(TValue);
        }
    }
}
