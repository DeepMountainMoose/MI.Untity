using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; } = "/";

        /// <summary>默认3000ms</summary>
        public int ContinuationTimeout { get; set; } = 3000;

        /// <summary>默认2000ms</summary>
        public int SocketTimeout { get; set; } = 2000;

        /// <summary>使用TuhuNotification的生产者数量，默认10</summary>
        public byte NotificationProducerCount { get; set; } = 10;

        public IConnectionFactory GetFactory() => new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            UseBackgroundThreadsForIO = true,
            HostName = HostName,
            UserName = UserName,
            Password = Password,
            VirtualHost = VirtualHost,
            ContinuationTimeout = TimeSpan.FromMilliseconds(ContinuationTimeout),
            HandshakeContinuationTimeout = TimeSpan.FromMilliseconds(ContinuationTimeout),
            RequestedConnectionTimeout = SocketTimeout,
            SocketReadTimeout = SocketTimeout,
            SocketWriteTimeout = SocketTimeout,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
            DispatchConsumersAsync = true
        };
    }
}
