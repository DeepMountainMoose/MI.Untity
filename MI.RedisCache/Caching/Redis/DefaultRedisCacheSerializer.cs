using MI.Core.Dependency;
using MI.Core.Extensions;
using MI.Core.Runtime.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    /// <summary>
    ///     使用Json实现的默认序列化器
    /// </summary>
    public class DefaultRedisCacheSerializer : IRedisCacheSerializer, ITransientDependency
    {
        /// <summary>
        ///     从RedisValue进行对象的反序列化
        /// </summary>
        /// <param name="objbyte">从Redis服务器读出来的对象的字符串表示形式</param>
        /// <returns>返回新构造的对象</returns>
        /// <seealso cref="Serialize" />
        public virtual object Deserialize(RedisValue objbyte)
        {
            var serializerSettings = new JsonSerializerSettings();

            var cacheData = CacheData.Deserialize(objbyte);

            return cacheData.Payload.FromJsonString(
                Type.GetType(cacheData.Type, true, true),
                serializerSettings);
        }

        /// <summary>
        ///     从RedisValue进行对象的反序列化
        /// </summary>
        /// <param name="objbyte">从Redis服务器读出来的对象的字符串表示形式</param>
        /// <returns>返回新构造的对象</returns>
        /// <seealso cref="Serialize" />
        public virtual T Deserialize<T>(RedisValue objbyte)
        {
            var serializerSettings = new JsonSerializerSettings();

            var cacheData = CacheData.Deserialize(objbyte);

            return (T)cacheData.Payload.FromJsonString(
                typeof(T),
                serializerSettings);
        }

        /// <summary>
        ///     将一个对象序列化为字符串
        /// </summary>
        /// <param name="value">需要被序列化的实例</param>
        /// <param name="type">对象的类型</param>
        /// <returns>将对象实例转变为一个可存入Redis的字符串</returns>
        /// <seealso cref="Deserialize" />
        public virtual string Serialize(object value, Type type)
        {
            return JsonConvert.SerializeObject(CacheData.Serialize(value));
        }
    }
}
