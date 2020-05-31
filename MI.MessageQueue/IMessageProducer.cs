using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public interface IMessageProducer
    {
        /// <summary>发送消息</summary>
        /// <param name="message">消息</param>
        void Send([NotNull]Message message);

        /// <summary>发送消息</summary>
        /// <param name="message">消息</param>
        void Send(string exchangeName, [NotNull]Message message);

        /// <summary>发送消息</summary>
        /// <param name="message">消息</param>
        /// <param name="cancellationToken">token</param>
        Task SendAsync([NotNull]Message message, CancellationToken cancellationToken);

        /// <summary>发送消息</summary>
        /// <param name="message">消息</param>
        /// <param name="cancellationToken">token</param>
        Task SendAsync([NotNull]Message message, string exchangeName, CancellationToken cancellationToken);

        /// <summary>批量发送消息</summary>
        /// <param name="messages">多个消息</param>
        void Send([NotNull]IReadOnlyCollection<Message> messages);

        /// <summary>批量发送消息</summary>
        /// <param name="messages">多个消息</param>
        /// <param name="cancellationToken">token</param>
        Task SendAsync([NotNull]IReadOnlyCollection<Message> messages, CancellationToken cancellationToken);
    }
}
