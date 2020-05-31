using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class RabbitMQConsumer : RabbitMQChannel
    {
        private readonly string _applicationName;

        #region 属性
        public string ConsumerName { get; }

        public Action<Exception> OnError;
        #endregion

        #region ctor
        internal RabbitMQConsumer([NotNull]IModel channel, string applicationName) : base(channel) => _applicationName = applicationName;
        internal RabbitMQConsumer([NotNull]IModel channel, string applicationName, string consumerName) : this(channel, applicationName) => ConsumerName = consumerName;

        /// <summary>声明一个临时队列，在Dispose之后将被删除</summary>
        /// <returns>临时队列名称</returns>
        public string TemporaryQueueBind([NotNull]string exchangeName, [NotNull]string routingKey)
        {
            var ok = Channel.QueueDeclare();
            Channel.QueueBind(ok.QueueName, exchangeName, routingKey);
            return ok.QueueName;
        }
        #endregion

        protected SemaphoreSlim Semaphore { get; private set; }

        protected CancellationTokenSource Cts { get; private set; }

        protected IBasicConsumer Consumer { get; private set; }

        protected void Subscribe(string queueName, ushort concurrencySize, bool noAck, IServiceProvider provider, Action<IServiceProvider, IReceivedMessage> func) =>
    SubscribeAsync(queueName, concurrencySize, noAck, provider, (p, message) =>
    {
        func(p, message);

        return Task.CompletedTask;
    });

        protected void SubscribeAsync(string queueName, ushort concurrencySize, bool noAck, IServiceProvider provider, Func<IServiceProvider, IReceivedMessage, Task> func)
        {
            if (Semaphore != null)
                throw new Exception("订阅已启动或未正常停止");

            Semaphore = new SemaphoreSlim(0);
            Cts = new CancellationTokenSource();

            Channel.BasicQos(0, concurrencySize, false);

            var consumer = new AsyncEventingBasicConsumer(Channel);

            if (provider == null)
                consumer.Received += (sender, args) => func(null, new DeliverMessage(args));
            else
                consumer.Received += async (sender, args) =>
                 {
                     using (var scope = provider.CreateScopeWithAccessor())
                         await func(scope.ServiceProvider, new DeliverMessage(args)).ConfigureAwait(false);
                 };
            Consumer = consumer;

            Channel.BasicConsume(queueName, noAck, $"{Environment.MachineName}|{_applicationName}|{ConsumerName}", Consumer);
        }

        #region Dispose

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            base.Dispose(disposing);

            if (disposing)
            {
                if (Semaphore != null)
                {
                    Cts.Cancel();

                    while (Semaphore.CurrentCount > 0)
                    {
                        Semaphore.Wait(); //等待所有异步完成
                    }

                    Cts.Dispose();

                    Semaphore.Dispose();

                    Cts = null;
                    Semaphore = null;
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
