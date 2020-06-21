using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace MI.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>将键值集合转换成字符串，key1=value1&amp;key2=value2，k/v会编码</summary>
        /// <param name="source">数据源</param>
        /// <returns>字符串</returns>
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> source)
        {
            if (source == null)
                return null;

            var sb = new StringBuilder(8192);

            foreach (var item in source)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    continue;

                sb.Append("&");
                sb.Append(WebUtility.UrlEncode(item.Key));
                sb.Append("=");
                if (item.Value != null)
                    sb.Append(WebUtility.UrlEncode(item.Value));
            }

            return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : "";
        }

        /// <summary>
        /// 字典移除指定Key（检查是否存在）
        /// </summary>
        public static TV GetAndRemove<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
        {
            if (dictionary.TryGetValue(key, out var value))
                dictionary.Remove(key);

            return value;
        }

        #region TryAdd/GetOrAdd
        /// <summary>
        /// 安全添加值（防止并发）
        /// </summary>
        public static bool TryAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] object syncLock, TKey key, TValue value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (syncLock == null) throw new ArgumentNullException(nameof(syncLock));

            if (dictionary.ContainsKey(key))
                return false;

            lock (syncLock)
            {
                if (dictionary.ContainsKey(key))
                    return false;

                dictionary[key] = value;
            }

            return true;
        }

        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的TryAdd的valueFactory可能会执行多次）</summary>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, [NotNull] object syncLock, TKey key, [NotNull] Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (syncLock == null) throw new ArgumentNullException(nameof(syncLock));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (dictionary.ContainsKey(key))
                return false;

            lock (syncLock)
            {
                if (dictionary.ContainsKey(key))
                    return false;

                dictionary[key] = valueFactory(key);
            }

            return true;
        }

        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的GetOrAdd的valueFactory可能会执行多次）</summary>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] object syncLock, TKey key, [NotNull] Func<TKey, TValue> valueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (syncLock == null) throw new ArgumentNullException(nameof(syncLock));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (!dictionary.TryGetValue(key, out var value))
                lock (syncLock)
                    if (!dictionary.TryGetValue(key, out value))
                        dictionary[key] = value = valueFactory(key);

            return value;
        }
        #endregion

        #region TryAddAsync/GetOrAddAsync
        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的TryAdd的valueFactory可能会执行多次）</summary>
        public static async Task<bool> TryAddAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            [NotNull] AsyncLock asyncLock, TKey key, [NotNull] Func<TKey, Task<TValue>> valueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (asyncLock == null) throw new ArgumentNullException(nameof(asyncLock));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (dictionary.ContainsKey(key)) return true;

            using (await asyncLock.LockAsync().ConfigureAwait(false))
            {
                if (dictionary.ContainsKey(key))
                    return false;

                dictionary[key] = await valueFactory(key).ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的TryAdd的valueFactory可能会执行多次）</summary>
        public static async Task<bool> TryAddAsync<TKey, TArg, TValue>(this IDictionary<TKey, TValue> dictionary,
            [NotNull] AsyncLock asyncLock, TKey key, [NotNull] Func<TKey, TArg, Task<TValue>> valueFactory, TArg factoryArgument)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (asyncLock == null) throw new ArgumentNullException(nameof(asyncLock));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (dictionary.ContainsKey(key))
                return false;

            using (await asyncLock.LockAsync().ConfigureAwait(false))
            {
                if (dictionary.ContainsKey(key))
                    return false;

                dictionary[key] = await valueFactory(key, factoryArgument).ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的GetOrAdd的valueFactory可能会执行多次）</summary>
        public static async Task<TValue> GetOrAddAsync<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] AsyncLock asyncLock, TKey key, [NotNull] Func<TKey, Task<TValue>> valueFactory)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (asyncLock == null) throw new ArgumentNullException(nameof(asyncLock));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (dictionary.TryGetValue(key, out var value)) return value;

            using (await asyncLock.LockAsync().ConfigureAwait(false))
                if (!dictionary.TryGetValue(key, out value))
                    dictionary[key] = value = await valueFactory(key).ConfigureAwait(false);

            return value;
        }

        /// <summary>安全添加值，valueFactory确定只会执行一次（ConcurrentDictionary的GetOrAdd的valueFactory可能会执行多次）</summary>
        public static async Task<TValue> GetOrAddAsync<TKey, TArg, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] AsyncLock asyncLock, TKey key, Func<TKey, TArg, Task<TValue>> valueFactory, [NotNull] TArg factoryArgument)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (asyncLock == null) throw new ArgumentNullException(nameof(asyncLock));
            if (factoryArgument == null) throw new ArgumentNullException(nameof(factoryArgument));

            if (dictionary.TryGetValue(key, out var value)) return value;

            using (await asyncLock.LockAsync().ConfigureAwait(false))
                if (!dictionary.TryGetValue(key, out value))
                    dictionary[key] = value = await valueFactory(key, factoryArgument).ConfigureAwait(false);

            return value;
        }
        #endregion


    }
}
