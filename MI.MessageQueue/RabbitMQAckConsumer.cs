using JetBrains.Annotations;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class RabbitMQAckConsumer : RabbitMQConsumer
    {
        internal RabbitMQAckConsumer(IModel channel, string applicationName) : base(channel, applicationName) { }

        internal RabbitMQAckConsumer(IModel channel, string consumerName, string applicationName) : base(channel, applicationName, consumerName) { }

        public bool Get(string queueName, Func<IReceivedMessage, bool> onReceived)
        {
            var msgResponse = Channel.BasicGet(queueName, false);
            if (msgResponse == null)
                return false;

            try
            {
                if (onReceived(new BasicGetMessage(msgResponse)))
                    BasicAck(msgResponse.DeliveryTag);
                else
                    BasicNack(msgResponse.DeliveryTag);

                return true;
            }
            catch when (BasicNack(msgResponse.DeliveryTag))
            {
                throw;
            }

        }

        /// <summary>尝试获取一条消息</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="onReceivedAsync">获取到</param>
        /// <returns>是否获取到消息</returns>
        public async Task<bool> GetAsync(string queueName, Func<IReceivedMessage, Task<bool>> onReceivedAsync)
        {
            var msgResponse = Channel.BasicGet(queueName, false);
            if (msgResponse == null)
                return false;

            try
            {
                if (await onReceivedAsync(new BasicGetMessage(msgResponse)).ConfigureAwait(false))
                    BasicAck(msgResponse.DeliveryTag);
                else
                    BasicNack(msgResponse.DeliveryTag);

                return true;
            }
            catch when (BasicNack(msgResponse.DeliveryTag))
            {
                throw;
            }
        }

        #region Subscribe

        /// <summary>需回复，同步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="onReceived">回调</param>
        public void Subscribe([NotNull]string queueName, [NotNull]Func<IReceivedMessage, bool> onReceived) => Subscribe(queueName, 1, onReceived);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="onReceived">回调，消息是否处理完成</param>
        public void Subscribe([NotNull]string queueName, byte concurrencySize, [NotNull]Func<IReceivedMessage, bool> onReceived) =>
            Subscribe(queueName, concurrencySize, false, null, (_, message) =>
            {
                Semaphore.Release();
                try
                {
                    if (onReceived(message))
                        BasicAck(message.DeliveryTag);
                    else
                        BasicNack(message.DeliveryTag);
                }
                catch (Exception ex)
                {
                    BasicNack(message.DeliveryTag);

                    OnError?.Invoke(ex);
                }
                finally
                {
                    if (!Cts.IsCancellationRequested)
                        Semaphore.Wait();
                }
            });

        /// <summary>需回复，同步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="provider"></param>
        /// <param name="onReceived">回调</param>
        public void Subscribe([NotNull]string queueName, [NotNull] IServiceProvider provider, [NotNull]Func<IServiceProvider, IReceivedMessage, bool> onReceived) =>
            Subscribe(queueName, 1, provider, onReceived);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="provider"></param>
        /// <param name="onReceived">回调，消息是否处理完成</param>
        public void Subscribe([NotNull] string queueName, byte concurrencySize, [NotNull] IServiceProvider provider, [NotNull] Func<IServiceProvider, IReceivedMessage, bool> onReceived) =>
            Subscribe(queueName, concurrencySize, false,
                provider ?? throw new ArgumentNullException(nameof(provider)),
                (p, message) =>
                {
                    Semaphore.Release();
                    try
                    {
                        if (onReceived(p, message))
                            BasicAck(message.DeliveryTag);
                        else
                            BasicNack(message.DeliveryTag);
                    }
                    catch (Exception ex)
                    {
                        BasicNack(message.DeliveryTag);

                        OnError?.Invoke(ex);
                    }
                    finally
                    {
                        if (!Cts.IsCancellationRequested)
                            Semaphore.Wait();
                    }
                });

        #endregion

        #region SubscribeAsync

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="onReceivedAsync">回调，消息是否处理完成</param>
        public void SubscribeAsync([NotNull]string queueName, [NotNull]Func<IReceivedMessage, Task<bool>> onReceivedAsync) => SubscribeAsync(queueName, 1, onReceivedAsync);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="onReceivedAsync">回调</param>
        public void SubscribeAsync([NotNull]string queueName, byte concurrencySize, [NotNull]Func<IReceivedMessage, Task<bool>> onReceivedAsync) =>
            SubscribeAsync(queueName, concurrencySize, false, null, async (_, message) =>
            {
                Semaphore.Release();
                try
                {
                    if (await onReceivedAsync(message).ConfigureAwait(false))
                        BasicAck(message.DeliveryTag);
                    else
                        BasicNack(message.DeliveryTag);
                }
                catch (Exception ex)
                {
                    BasicNack(message.DeliveryTag);

                    OnError?.Invoke(ex);
                }
                finally
                {
                    if (!Cts.IsCancellationRequested)
                        await Semaphore.WaitAsync().ConfigureAwait(false);
                }
            });

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="provider"></param>
        /// <param name="onReceivedAsync">回调，消息是否处理完成</param>
        public void SubscribeAsync([NotNull]string queueName, [NotNull] IServiceProvider provider, [NotNull]Func<IServiceProvider, IReceivedMessage, Task<bool>> onReceivedAsync) => SubscribeAsync(queueName, 1, provider, onReceivedAsync);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="provider"></param>
        /// <param name="onReceivedAsync">回调</param>
        public void SubscribeAsync([NotNull] string queueName, byte concurrencySize, [NotNull] IServiceProvider provider, [NotNull] Func<IServiceProvider, IReceivedMessage, Task<bool>> onReceivedAsync) =>
            SubscribeAsync(queueName, concurrencySize, false,
                provider ?? throw new ArgumentNullException(nameof(provider)),
                async (p, message) =>
                {
                    Semaphore.Release();
                    try
                    {
                        if (await onReceivedAsync(p, message).ConfigureAwait(false))
                            BasicAck(message.DeliveryTag);
                        else
                            BasicNack(message.DeliveryTag);
                    }
                    catch (Exception ex)
                    {
                        BasicNack(message.DeliveryTag);

                        OnError?.Invoke(ex);
                    }
                    finally
                    {
                        if (!Cts.IsCancellationRequested)
                            await Semaphore.WaitAsync().ConfigureAwait(false);
                    }
                });

        #endregion

        #region Method
        private bool BasicNack(ulong deliveryTag)
        {
            Channel.BasicNack(deliveryTag, false, true);
            return false;
        }

        private bool BasicAck(ulong deliveryTag)
        {
            Channel.BasicAck(deliveryTag, false);
            return true;
        } 
        #endregion
    }
}
