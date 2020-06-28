using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    /// <summary>
    ///     用于处理序列化和反序列化到Redis缓存的服务
    /// </summary>
    public interface IRedisCacheSerializer
    {
        /// <summary>
        ///     从RedisValue进行对象的反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的对象</typeparam>
        /// <param name="objbyte">从Redis服务器读出来的对象的字符串表示形式</param>
        /// <returns></returns>
        T Deserialize<T>(RedisValue objbyte);

        /// <summary>
        ///     从RedisValue进行对象的反序列化
        /// </summary>
        /// <param name="objbyte">从Redis服务器读出来的对象的字符串表示形式</param>
        /// <returns>返回新构造的对象</returns>
        /// <seealso cref="Serialize" />
        object Deserialize(RedisValue objbyte);

        /// <summary>
        ///     将一个对象序列化为字符串
        /// </summary>
        /// <param name="value">需要被序列化的实例</param>
        /// <param name="type">对象的类型</param>
        /// <returns>将对象实例转变为一个可存入Redis的字符串</returns>
        /// <seealso cref="Deserialize" />
        string Serialize(object value, Type type);
    }
}
