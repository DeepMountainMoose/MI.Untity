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
    public class MINotification : IMINotification
    {
        private static readonly string ExchangeName = "topic.notificationExchage";
        private readonly IMessageProducer _producer;

        public MINotification(RabbitMQClient client, string exchangeName, IOptions<RabbitMQOptions> options)
        {
            exchangeName = string.IsNullOrEmpty(exchangeName) ? ExchangeName : exchangeName;

            var producer = client.CreateProducer(exchangeName);

            producer.ExchangeDeclare(exchangeName, ExchangeType.Topic);

            _producer = producer;
        }

        #region SendNotification
        public void SendNotification(string notificationKey, object data) => _producer.Send(notificationKey, data);

        public void SendNotification(string notificationKey, string exchangeName, object data) => _producer.Send(exchangeName, notificationKey, data);

        public void SendNotification(string notificationKey, object data, MessagePriority priority) => _producer.Send(new Message { To = notificationKey, Body = data, Priority = priority });

        public void SendNotification(string notificationKey, object data, int delay) =>
            _producer.Send(new Message { To = notificationKey, Body = data, Delay = delay });

        public void SendNotification(string notificationKey, object data, MessagePriority priority, int delay) =>
            _producer.Send(new Message { To = notificationKey, Body = data, Priority = priority, Delay = delay });

        public void SendNotification([NotNull] IEnumerable<ValueTuple<string, object>> messages) =>
            _producer.Send(messages.Select(m => new Message { To = m.Item1, Body = m.Item2 }).ToArray());
        #endregion

        #region SendNotificationAsync
        public async Task SendNotificationAsync([NotNull] string notificationKey, object data, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(new Message { To = notificationKey, Body = data }, cancellationToken);
        }

        public async Task SendNotificationAsync([NotNull] string exchangeName, string notificationKey, object data, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(new Message { To = notificationKey, Body = data }, exchangeName, cancellationToken);
        }

        public async Task SendNotificationAsync([NotNull] string notificationKey, object data, int delay, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(new Message { To = notificationKey, Body = data, Delay = delay }, cancellationToken);
        }

        public async Task SendNotificationAsync([NotNull] string notificationKey, object data, MessagePriority priority, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(new Message { To = notificationKey, Body = data, Priority = priority }, cancellationToken);
        }

        public async Task SendNotificationAsync([NotNull] string notificationKey, object data, MessagePriority priority, int delay, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(new Message { To = notificationKey, Body = data, Priority = priority, Delay = delay }, cancellationToken);
        }

        public async Task SendNotificationAsync([NotNull] IEnumerable<(string, object)> messages, CancellationToken cancellationToken = default)
        {
            await _producer.SendAsync(messages.Select(m => new Message { To = m.Item1, Body = m.Item2 }).ToArray(), cancellationToken);
        }
        #endregion
    }
}
