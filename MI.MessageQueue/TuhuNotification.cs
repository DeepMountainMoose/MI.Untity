using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public static class TuhuNotification
    {
        private static readonly ITuhuNotification Notification = ServiceResolver.CreateInstance<DefaultTuhuNotification>();

        #region SendNotification

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        public static void SendNotification(string notificationKey, object data) =>
            Notification.SendNotification(notificationKey, data);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="delay">延迟毫秒数</param>
        public static void SendNotification(string notificationKey, string exchangeName, object data, int delay) =>
            Notification.SendNotification(notificationKey, exchangeName, data);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        public static void SendNotification(string notificationKey, object data, MessagePriority priority) =>
            Notification.SendNotification(notificationKey, data, priority);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="delay">延迟毫秒数</param>
        public static void SendNotification(string notificationKey, object data, int delay) =>
            Notification.SendNotification(notificationKey, data, delay);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="delay">延迟毫秒数</param>
        public static void SendNotification(string notificationKey, object data, MessagePriority priority, int delay) =>
            Notification.SendNotification(notificationKey, data, priority, delay);
        #endregion

        #region SendNotificationAsync

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        public static Task SendNotificationAsync(string notificationKey, object data) =>
            Notification.SendNotificationAsync(notificationKey, data);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        public static Task SendNotificationAsync(string notificationKey, object data, MessagePriority priority) =>
            Notification.SendNotificationAsync(notificationKey, data, priority);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="delay">延迟毫秒数</param>
        public static Task SendNotificationAsync(string notificationKey, object data, int delay) =>
            Notification.SendNotificationAsync(notificationKey, data, delay);

        /// <summary>发送更新通知。http://blog.csdn.net/anzhsoft/article/details/19633079</summary>
        /// <param name="notificationKey">通知Key</param>
        /// <param name="data">内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="delay">延迟毫秒数</param>
        public static Task SendNotificationAsync(string notificationKey, object data, MessagePriority priority, int delay) =>
            Notification.SendNotificationAsync(notificationKey, data, priority, delay);

        #endregion
    }
}
