using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public interface ITuhuNotification
    {
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        void SendNotification([NotNull]string notificationKey, object data);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="delay">延迟毫秒数</param>
        void SendNotification([NotNull]string notificationKey, object data, int delay);
        /// <summary>单条发送消息到指定交换器</summary>
        /// <param name="notificationKey">通知Key</param>
        /// /// <param name="exchangeName">交换器名称</param>
        /// <param name="data">内容</param>
        void SendNotification([NotNull]string notificationKey, string exchangeName, object data);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        void SendNotification([NotNull]string notificationKey, object data, MessagePriority priority);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="delay">延迟毫秒数</param>
        void SendNotification([NotNull]string notificationKey, object data, MessagePriority priority, int delay);
        /// <summary>批量发送消息</summary>
        void SendNotification([NotNull]IEnumerable<ValueTuple<string, object>> messages);

        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        Task SendNotificationAsync([NotNull]string notificationKey, object data, CancellationToken cancellationToken = default);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="delay">延迟毫秒数</param>
        Task SendNotificationAsync([NotNull]string notificationKey, object data, int delay, CancellationToken cancellationToken = default);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        Task SendNotificationAsync([NotNull]string notificationKey, object data, MessagePriority priority, CancellationToken cancellationToken = default);
        /// <summary>单条发送消息</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="delay">延迟毫秒数</param>
        Task SendNotificationAsync([NotNull]string notificationKey, object data, MessagePriority priority, int delay, CancellationToken cancellationToken = default);
        /// <summary>批量发送消息</summary>
        Task SendNotificationAsync([NotNull]IEnumerable<ValueTuple<string, object>> messages, CancellationToken cancellationToken = default);
    }
}
