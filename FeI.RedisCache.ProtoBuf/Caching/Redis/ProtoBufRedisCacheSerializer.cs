using MI.RedisCache.Caching.Redis;
using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FeI.RedisCache.ProtoBuf.Caching.Redis
{
    public class ProtoBufRedisCacheSerializer : DefaultRedisCacheSerializer
    {
        private const string TypeSeperator = "|";
        private const string ProtoBufPrefix = "PB^";

        /// <summary>
        ///     反序列化信息
        /// </summary>
        /// <param name="objbyte">从Redis读取出来的字符串信息</param>
        /// <returns>返回新创建的对象</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public override object Deserialize(RedisValue objbyte)
        {
            string serializedObj = objbyte;
            if (!serializedObj.StartsWith(ProtoBufPrefix))
            {
                return base.Deserialize(objbyte);
            }

            serializedObj = serializedObj.Substring(ProtoBufPrefix.Length);
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator, StringComparison.OrdinalIgnoreCase);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex), true, true);
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
            var byteAfter64 = Convert.FromBase64String(serialized);

            using (var memoryStream = new MemoryStream(byteAfter64))
            {
                return Serializer.Deserialize(type, memoryStream);
            }
        }

        /// <summary>
        ///     反序列化信息
        /// </summary>
        /// <param name="objbyte">从Redis读取出来的字符串信息</param>
        /// <returns>返回新创建的对象</returns>
        /// <seealso cref="IRedisCacheSerializer.Serialize" />
        public override T Deserialize<T>(RedisValue objbyte)
        {
            string serializedObj = objbyte;
            if (!serializedObj.StartsWith(ProtoBufPrefix))
            {
                return base.Deserialize<T>(objbyte);
            }

            serializedObj = serializedObj.Substring(ProtoBufPrefix.Length);
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator, StringComparison.OrdinalIgnoreCase);
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
            var byteAfter64 = Convert.FromBase64String(serialized);

            using (var memoryStream = new MemoryStream(byteAfter64))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        /// <summary>
        ///     将给定对象序列化为字符串
        /// </summary>
        /// <param name="value">将要序列化的对象</param>
        /// <param name="type">序列化的对象的类型</param>
        /// <returns>可以存储到Redis的字符串</returns>
        /// <seealso cref="IRedisCacheSerializer.Deserialize" />
        public override string Serialize(object value, Type type)
        {
            if (!type.GetTypeInfo().IsDefined(typeof(ProtoContractAttribute), false))
            {
                return base.Serialize(value, type);
            }

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, value);
                var byteArray = memoryStream.ToArray();
                var serialized = Convert.ToBase64String(byteArray, 0, byteArray.Length);
                return $"{ProtoBufPrefix}{type.AssemblyQualifiedName}{TypeSeperator}{serialized}";
            }
        }
    }
}
