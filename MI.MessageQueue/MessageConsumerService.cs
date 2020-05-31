using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    /// <summary>队列消费服务</summary>
    public abstract class MessageConsumerService : IDisposable
    {
        public MessageConsumerService(ILogger logger) => Logger = logger;

        public ILogger Logger { get; }

        public RabbitMQConsumer Consumer { get; private set; }

        public void Register(RabbitMQClient client)
        {
            Consumer = BeginConsume(GetType().Name, client);

            if (Consumer != null)
                Consumer.OnError += ex => Logger.LogError(ex.Message, ex);
        }

        public abstract RabbitMQConsumer BeginConsume(string name, RabbitMQClient client);

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            Consumer?.Dispose();

            _disposed = true;
        }

        ~MessageConsumerService()
        {
            Dispose(false);
        }
        #endregion

    }
}
