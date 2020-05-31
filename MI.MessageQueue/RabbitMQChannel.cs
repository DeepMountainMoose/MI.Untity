using JetBrains.Annotations;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public abstract class RabbitMQChannel : IDisposable
    {
        protected RabbitMQChannel([NotNull]IModel channel) { Channel = channel; }
        public IModel Channel { get; }

        #region ExchangeDeclare

        public void ExchangeDeclare([NotNull]string exchangeName) => ExchangeDeclare(exchangeName, ExchangeType.Direct);

        public void ExchangeDeclare([NotNull]string exchangeName, bool isDelay) => ExchangeDeclare(exchangeName, ExchangeType.Direct, isDelay);

        public void ExchangeDeclare([NotNull]string exchangeName, ExchangeType type) => ExchangeDeclare(exchangeName, type, false);

        public void ExchangeDeclare([NotNull]string exchangeName, ExchangeType type, bool isDelay)
        {
            if (isDelay)
                Channel.ExchangeDeclare(exchangeName, "x-delayed-message", true, false, new Dictionary<string, object> { { "x-delayed-type", GetExchangeTypeName(type) } });
            else
                Channel.ExchangeDeclare(exchangeName, GetExchangeTypeName(type), true, false, null);
        }


        private static string GetExchangeTypeName(ExchangeType type)
        {
            switch (type)
            {
                case ExchangeType.Direct:
                    return RabbitMQ.Client.ExchangeType.Direct;
                case ExchangeType.Fanout:
                    return RabbitMQ.Client.ExchangeType.Fanout;
                case ExchangeType.Topic:
                    return RabbitMQ.Client.ExchangeType.Topic;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        #endregion

        #region QueueBind

        /// <summary>声明一个队列并将队列绑定到exchange</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchangeName">交换机名称</param>
        public QueueDeclareOk QueueBind([NotNull]string queueName, [NotNull]string exchangeName) => QueueBind(queueName, exchangeName, queueName, null);

        /// <summary>声明一个队列并将队列绑定到exchange</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="priority">优先级数量，priority + 1</param>
        public QueueDeclareOk QueueBind([NotNull]string queueName, [NotNull]string exchangeName, MessagePriority priority) => QueueBind(queueName, exchangeName, queueName, priority);

        /// <summary>声明一个队列并将队列绑定到exchange。一个routingKey绑定多个Queue，一个Queue绑定多个routingKey</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">routingKey</param>
        public QueueDeclareOk QueueBind([NotNull]string queueName, [NotNull]string exchangeName, [NotNull]string routingKey) => QueueBind(queueName, exchangeName, routingKey, null);

        /// <summary>声明一个队列并将队列绑定到exchange。一个routingKey绑定多个Queue，一个Queue绑定多个routingKey</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">routingKey</param>
        /// <param name="priority">优先级数量，priority + 1</param>
        public QueueDeclareOk QueueBind([NotNull]string queueName, [NotNull]string exchangeName, [NotNull]string routingKey, MessagePriority priority) => QueueBind(queueName, exchangeName, routingKey, priority < MessagePriority.Lowest ? null : new Dictionary<string, object> { { "x-max-priority", priority > MessagePriority.Highest ? 10 : (byte)priority + 1 } });

        /// <summary>声明一个队列并将队列绑定到exchange。一个routingKey绑定多个Queue，一个Queue绑定多个routingKey</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">routingKey</param>
        public QueueDeclareOk QueueBind([NotNull]string queueName, [NotNull]string exchangeName, [NotNull]string routingKey, IDictionary<string, object> arguments)
        {
            var result = Channel.QueueDeclare(queueName, true, false, false, arguments);
            Channel.QueueBind(queueName, exchangeName, routingKey);

            return result;
        } 
        #endregion

        #region Dispose
        public void Dispose()
        {

        }

        bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //释放托管资源，比如将对象设置为null
            }

            //释放非托管资源
            if (Channel != null)
            {
                Channel.Close();
            }
        } 
        #endregion
    }


    public enum ExchangeType : byte
    {
        /// <summary>如果routingKey匹配，那么Message就会被传递到相应的queue中。http://blog.csdn.net/anzhsoft/article/details/19630147</summary>
        Direct = 1,
        /// <summary>会向所有响应的queue广播。http://blog.csdn.net/anzhsoft/article/details/19617305</summary>
        Fanout = 2,
        /// <summary>对key进行模式匹配，比如ab.* 可以传递到所有ab.*的queue。* (星号) 代表任意 一个单词；# (hash) 0个或者多个单词。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        Topic = 3
    }

    /// <summary>优先级</summary>
    [Serializable]
    public enum MessagePriority : byte
    {
        /// <summary>优先级0</summary>
        None = 0,
        /// <summary>优先级1</summary>
        Lowest = 1,
        /// <summary>优先级2</summary>
        AboveLowest = 2,
        /// <summary>优先级3</summary>
        Low = 3,
        /// <summary>优先级4</summary>
        BelowNormal = 4,
        /// <summary>优先级5</summary>
        Normal = 5,
        /// <summary>优先级6</summary>
        AboveNormal = 6,
        /// <summary>优先级7</summary>
        Hight = 7,
        /// <summary>优先级8</summary>
        BelowHighest = 8,
        /// <summary>优先级9</summary>
        Highest = 9
    }
}
