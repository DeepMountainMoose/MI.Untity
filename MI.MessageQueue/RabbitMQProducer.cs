using JetBrains.Annotations;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class RabbitMQProducer : RabbitMQChannel, IMessageProducer
    {
        private readonly Lazy<Policy> _syncPolicy;

        #region ctor
        protected internal RabbitMQProducer([NotNull]IModel channel, string exchangeName) : base(channel)
        {
            DataFormatter = new JsonDataFormatter();
            ExchangeName = exchangeName;
            IsPersistent = true;

            _syncPolicy = new Lazy<Policy>(() => Policy
                .Handle<OperationInterruptedException>()
                .WaitAndRetry(Retires, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, Retires - retryAttempt - 6))));
        }
        #endregion

        #region Properties
        /// <summary>数据格式化器，默认JsonDataFormatter</summary>
        public IDataFormatter DataFormatter { get; set; }

        /// <summary>是否持续化</summary>
        public bool IsPersistent { get; set; }

        /// <summary>ExchangeName</summary>
        public string ExchangeName { get; }

        /// <summary>重试次数，默认7次，总间隔约4.2秒。重试间隔使用的是1.5的指数，如，第一次间隔Math.Pow(1.5, -5)，第二次间隔Math.Pow(1.5, -4)</summary>
        public byte Retires { get; set; } = 7;
        #endregion

        #region Send
        protected virtual IBasicProperties CreateBasicProperties(Message message)
        {
            var basicProperties = Channel.CreateBasicProperties();

            if (!string.IsNullOrEmpty(message.AppId))
                basicProperties.AppId = message.AppId;

            if (!string.IsNullOrEmpty(message.MessageId))
                basicProperties.MessageId = message.MessageId;

            basicProperties.DeliveryMode = IsPersistent ? (byte)2 : (byte)1;

            if (message.Priority != null)
            {
                if (message.Priority > MessagePriority.Highest)
                    throw new ArgumentOutOfRangeException(nameof(message.Priority), message.Priority, "最大值为10");

                basicProperties.Priority = (byte)message.Priority;
            }
            basicProperties.Headers = message.Headers == null ? new Dictionary<string, object>() : new Dictionary<string, object>(message.Headers);

            if (message.Delay > 0)
                basicProperties.Headers["x-delay"] = message.Delay;

            if (message.Now != null)
                basicProperties.Timestamp = new AmqpTimestamp(message.Now.Value.ToUnixTimeMilliseconds());

            return basicProperties;
        }

        public virtual void Send(Message message) =>
    Send(message.To, message.Body, CreateBasicProperties(message));

        public virtual Task SendAsync(Message message, CancellationToken cancellationToken) =>
            SendAsync(message.To, message.Body, CreateBasicProperties(message), cancellationToken);

        /// <summary>发送消息</summary>
        protected virtual void Send(string routingKey, object message, IBasicProperties basicProperties) =>
            _syncPolicy.Value.Execute(() => InternalSend(routingKey, message, basicProperties));

        protected virtual void Send(string exchangeName, string routingKey, object message, IBasicProperties basicProperties) =>
    _syncPolicy.Value.Execute(() => InternalSend(exchangeName, routingKey, message, basicProperties));

        /// <summary>发送消息</summary>
        protected virtual Task SendAsync(string routingKey, object message, IBasicProperties basicProperties, CancellationToken cancellationToken)
        {
            Send(routingKey, message, basicProperties);

            return Task.CompletedTask;
        }
        private void InternalSend(string routingKey, object message, IBasicProperties basicProperties) =>
            Channel.BasicPublish(ExchangeName, routingKey, basicProperties, DataFormatter.Serialize(message));

        private void InternalSend(string exchangeName, string routingKey, object message, IBasicProperties basicProperties) =>
    Channel.BasicPublish(exchangeName, routingKey, basicProperties, DataFormatter.Serialize(message));

        /// <inheritdoc />
        public virtual void Send(IReadOnlyCollection<Message> messages)
        {
            if (messages == null || messages.Count == 0) throw new ArgumentNullException(nameof(messages));

            _syncPolicy.Value.Execute(() => InternalSend(messages));
        }

        public virtual Task SendAsync(IReadOnlyCollection<Message> messages, CancellationToken token)
        {
            Send(messages);

            return Task.CompletedTask;
        }

        private void InternalSend(IEnumerable<Message> messages)
        {
            var basicPublishBatch = Channel.CreateBasicPublishBatch();

            foreach (var message in messages)
                basicPublishBatch.Add(ExchangeName, message.To, false, CreateBasicProperties(message), DataFormatter.Serialize(message.Body));

            basicPublishBatch.Publish();
        }

        public void Send(string exchangeName, [NotNull] Message message)
        {
            Send(exchangeName, message.To, message.Body, CreateBasicProperties(message));
        }

        /// <summary>
        /// 发送消息（可指定交换器）
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="exchangeName">交换器</param>
        /// <param name="routingKey">routingKey</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        public virtual Task SendAsync(Message message, string exchangeName, CancellationToken cancellationToken)
        {
            _syncPolicy.Value.Execute(() => InternalSend(exchangeName, message.To, message.Body, CreateBasicProperties(message)));

            return Task.CompletedTask;
        }
        #endregion
    }
}
