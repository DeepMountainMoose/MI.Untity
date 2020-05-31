using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class DefaultTuhuNotification : ITuhuNotification, IDisposable
    {
        private static readonly string ExchangeName = "topic.notificationExchage";
        private readonly IMessageProducer _producer;

        public DefaultTuhuNotification(IRabbitMQClientFactory factory, IOptions<RabbitMQOptions> options) : this(factory.GetDefaultClient(), options) { }

        public DefaultTuhuNotification(IRabbitMQClientFactory factory, string exchangeName,IOptions<RabbitMQOptions> options) : this(factory.GetDefaultClient(), exchangeName,options) { }

        public DefaultTuhuNotification(RabbitMQClient client, IOptions<RabbitMQOptions> options)
        {
            var producer = client.CreateProducer(ExchangeName);

            producer.ExchangeDeclare(ExchangeName, ExchangeType.Topic);

            _producer = producer;
        }

        public DefaultTuhuNotification(RabbitMQClient client, string exchangeName, IOptions<RabbitMQOptions> options)
        {
            exchangeName = string.IsNullOrWhiteSpace(exchangeName) ? ExchangeName : exchangeName;

            var producer = client.CreateProducer(ExchangeName);

            producer.ExchangeDeclare(ExchangeName, ExchangeType.Topic);

            _producer = producer;
        }

        public void Dispose() => (_producer as IDisposable)?.Dispose();

        #region SendNotification
        public void SendNotification(string notificationKey, object data) => _producer.Send(notificationKey, data);

        public void SendNotification(string notificationKey,string exchangeName, object data) => _producer.Send(notificationKey, data);

        public void SendNotification(string notificationKey, object data, MessagePriority priority) => _producer.Send(new Message { To = notificationKey, Body = data, Priority = priority });

        public void SendNotification(string notificationKey, object data, int delay) =>
            _producer.Send(new Message { To = notificationKey, Body = data, Delay = delay });

        public void SendNotification(string notificationKey, object data, MessagePriority priority, int delay) =>
            _producer.Send(new Message { To = notificationKey, Body = data, Priority = priority, Delay = delay });

        public void SendNotification([NotNull] IEnumerable<ValueTuple<string, object>> messages) =>
            _producer.Send(messages.Select(m => new Message { To = m.Item1, Body = m.Item2 }).ToArray());
        #endregion

        #region SendNotificationAsync
        public Task SendNotificationAsync(string notificationKey, object data, CancellationToken cancellationToken) => _producer.SendAsync(new Message { To = notificationKey, Body = data }, cancellationToken);

        public Task SendNotificationAsync(string notificationKey, object data, MessagePriority priority, CancellationToken cancellationToken) => _producer.SendAsync(new Message { To = notificationKey, Body = data, Priority = priority }, cancellationToken);

        public Task SendNotificationAsync(string notificationKey, object data, int delay, CancellationToken cancellationToken) =>
            _producer.SendAsync(new Message { To = notificationKey, Body = data, Delay = delay }, cancellationToken);

        public Task SendNotificationAsync(string notificationKey, object data, MessagePriority priority, int delay, CancellationToken cancellationToken) =>
            _producer.SendAsync(new Message { To = notificationKey, Body = data, Priority = priority, Delay = delay }, cancellationToken);

        public Task SendNotificationAsync([NotNull] IEnumerable<ValueTuple<string, object>> messages, CancellationToken cancellationToken) =>
            _producer.SendAsync(messages.Select(m => new Message { To = m.Item1, Body = m.Item2 }).ToArray(), cancellationToken);
        #endregion
    }
}
