using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public static class MessageProducerExtensions
    {
        public static void Send([NotNull]this IMessageProducer producer, [NotNull]string to, [NotNull]object message) =>
            producer.Send(new Message { To = to, Body = message });

        public static Task SendAsync([NotNull]this IMessageProducer producer, [NotNull]string to, [NotNull]object message) =>
            producer.SendAsync(new Message { To = to, Body = message }, CancellationToken.None);

        public static Task SendAsync([NotNull]this IMessageProducer producer, [NotNull]Message message) =>
            producer.SendAsync(message, CancellationToken.None);

        public static void Send([NotNull]this IMessageProducer producer, string exchangeName, [NotNull]string to, [NotNull]object message) =>
            producer.Send(exchangeName, new Message { To = to, Body = message });
    }
}
