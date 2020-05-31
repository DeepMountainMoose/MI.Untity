using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class Message
    {
        public string AppId { get; set; }

        public string MessageId { get; set; }

        public IDictionary<string, object> Headers { get; set; }

        public MessagePriority? Priority { get; set; }

        /// <summary>毫秒</summary>
        public int Delay { get; set; }

        /// <summary>RoutingKey或Topic</summary>
        public string To { get; set; }

        public object Body { get; set; }

        public DateTimeOffset? Now { get; set; }
    }

    public interface IReceivedMessage
    {
        IBasicProperties BasicProperties { get; }

        byte[] Body { get; }

        ulong DeliveryTag { get; }

        string Exchange { get; }

        bool Redelivered { get; }

        string RoutingKey { get; }
    }

    internal class BasicGetMessage : IReceivedMessage
    {
        public BasicGetMessage(BasicGetResult result)
        {
            BasicProperties = result.BasicProperties;
            Body = result.Body;
            DeliveryTag = result.DeliveryTag;
            Exchange = result.Exchange;
            MessageCount = result.MessageCount;
            Redelivered = result.Redelivered;
            RoutingKey = result.RoutingKey;
        }

        public IBasicProperties BasicProperties { get; }

        public byte[] Body { get; }

        public ulong DeliveryTag { get; }

        public string Exchange { get; }

        public ulong MessageCount { get; }

        public bool Redelivered { get; }

        public string RoutingKey { get; }
    }

    internal class DeliverMessage : IReceivedMessage
    {
        public DeliverMessage(BasicDeliverEventArgs args)
        {
            BasicProperties = args.BasicProperties;
            Body = args.Body;
            ConsumerTag = args.ConsumerTag;
            DeliveryTag = args.DeliveryTag;
            Exchange = args.Exchange;
            Redelivered = args.Redelivered;
            RoutingKey = args.RoutingKey;
        }

        public IBasicProperties BasicProperties { get; }

        public byte[] Body { get; }

        public string ConsumerTag { get; }

        public ulong DeliveryTag { get; }

        public string Exchange { get; }

        public bool Redelivered { get; }

        public string RoutingKey { get; }
    }

    /// <summary>MessageReciveResult</summary>
    public static class MessageExtension
    {
        private static readonly IDataFormatter DefaultJsonDataFormater = new JsonDataFormatter();
        private static readonly IDataFormatter DefaultXmlDataFormater = new XmlDataFormatter();

        /// <summary>Json反序列化</summary>
        public static T Deserialize<T>(this IReceivedMessage result) => JsonDeserialize<T>(result);

        /// <summary>Json反序列化</summary>
        public static T JsonDeserialize<T>(this IReceivedMessage result) => DefaultJsonDataFormater.Deserialize<T>(result.Body);

        /// <summary>Json反序列化</summary>
        public static T JsonDeserialize<T>(this IReceivedMessage result, JsonSerializerSettings settings) => new JsonDataFormatter(settings).Deserialize<T>(result.Body);

        /// <summary>XML反序列化</summary>
        public static T XmlDeserialize<T>(this IReceivedMessage result) => DefaultXmlDataFormater.Deserialize<T>(result.Body);
    }
}
