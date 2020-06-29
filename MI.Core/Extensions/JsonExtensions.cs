using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     转换到Json字符串的扩展方法
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly IContractResolver camelCaseResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        ///     转换指定的类型到Json字符串
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var options = new JsonSerializerSettings();

            if (camelCase) options.ContractResolver = camelCaseResolver;

            if (indented) options.Formatting = Formatting.Indented;

            return JsonConvert.SerializeObject(obj, options);
        }

        /// <summary>
        ///     使用默认的<see cref="JsonSerializerSettings" />返回反序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value)
        {
            return value.FromJsonString<T>(new JsonSerializerSettings());
        }

        /// <summary>
        ///     使用自定义的<see cref="JsonSerializerSettings" />返回反序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string value, JsonSerializerSettings settings)
        {
            return value != null
                ? JsonConvert.DeserializeObject<T>(value, settings)
                : default(T);
        }

        /// <summary>
        ///     使用自定义的<see cref="JsonSerializerSettings" />返回指定类型的反序列化的对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static object FromJsonString(this string value, [NotNull] Type type, JsonSerializerSettings settings)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return value != null
                ? JsonConvert.DeserializeObject(value, type, settings)
                : null;
        }
    }
}
