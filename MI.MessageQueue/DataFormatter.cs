using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MI.MessageQueue
{
    public abstract class DataFormatter : IDataFormatter
    {
        private static readonly TypeInfo TaskInfo = typeof(Task).GetTypeInfo();

        /// <summary>反序列化</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns>对象</returns>
        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            var targetType = typeof(T);

            if (TaskInfo.IsAssignableFrom(targetType))
                throw new SerializationException("不支持Task序列化和反序列化");

            if (targetType == typeof(byte[]))
                return (T)(object)data;

            if (targetType == typeof(string))
                return (T)(object)Encoding.UTF8.GetString(data);

            return InternalDeserialize<T>(data);
        }

        /// <summary>序列化</summary>
        /// <param name="objData">对象</param>
        /// <returns>数据</returns>
        public byte[] Serialize(object objData)
        {
            if (objData is Task)
                throw new SerializationException("不支持Task序列化和反序列化");

            if (objData is byte[] data)
                return data;

            return objData is string s ? Encoding.UTF8.GetBytes(s) : InternalSerialize(objData);
        }

        protected abstract T InternalDeserialize<T>(byte[] data);

        protected abstract byte[] InternalSerialize(object objData);
    }

    public class JsonDataFormatter : DataFormatter
    {
        /// <summary>JsonSerializerSettings</summary>
        public JsonSerializerSettings Settings { get; set; }

        /// <inheritdoc />
        public JsonDataFormatter() : this(null) { }

        /// <summary>ctor</summary>
        /// <param name="settings">JsonSerializerSettings</param>
        public JsonDataFormatter(JsonSerializerSettings settings) { Settings = settings ?? new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }; }

        protected override T InternalDeserialize<T>(byte[] data)
        {
            var serializer = JsonSerializer.CreateDefault(Settings);

            using (var stream = new MemoryStream(data))
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            using (var jr = new JsonTextReader(sr))
                return serializer.Deserialize<T>(jr);
        }

        protected override byte[] InternalSerialize(object objData) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objData, Settings));
    }

    public class XmlDataFormatter : DataFormatter
    {
        protected override T InternalDeserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                return (T)xmlSerializer.Deserialize(stream);
            }
        }

        protected override byte[] InternalSerialize(object objData)
        {
            if (objData == null)
                return null;

            using (var stream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(objData.GetType());

                xmlSerializer.Serialize(stream, objData);

                return stream.ToArray();
            }
        }
    }
}
