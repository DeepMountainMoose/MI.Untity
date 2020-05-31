using JetBrains.Annotations;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    public class RabbitMQNoackConsumer : RabbitMQConsumer
    {
        #region ctor
        public RabbitMQNoackConsumer([NotNull]IModel channel, string applicationName) : base(channel, applicationName) { }

        public RabbitMQNoackConsumer([NotNull]IModel channel, string applicationName, string consumerName) : base(channel, applicationName, consumerName) { }
        #endregion

        /// <summary>尝试获取一条消息</summary>
        /// <param name="queueName">队列名称</param>
        /// <returns>是否获取到消息</returns>
        public IReceivedMessage Get(string queueName)
        {
            var msgResponse = Channel.BasicGet(queueName, true);
            if (msgResponse == null)
                return null;

            return new BasicGetMessage(msgResponse);
        }

        #region Subscribe
        /// <summary>无需回复，同步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="onReceived">回调。不管消息处理结果，都算处理完成</param>
        public void Subscribe([NotNull]string queueName, [NotNull]Action<IReceivedMessage> onReceived) =>
            Subscribe(queueName, 1, onReceived);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="onReceived">回调，消息是否处理完成</param>
        public void Subscribe([NotNull]string queueName, byte concurrencySize, [NotNull]Action<IReceivedMessage> onReceived) =>
            Subscribe(queueName, concurrencySize, true, null, (_, message) =>
            {
                Semaphore.Release();
                try
                {
                    onReceived(message);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex);
                }
                finally
                {
                    if (!Cts.IsCancellationRequested)
                        Semaphore.Wait();
                }
            });

        /// <summary>无需回复，同步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="provider"></param>
        /// <param name="onReceived">回调。不管消息处理结果，都算处理完成</param>
        public void Subscribe([NotNull]string queueName, IServiceProvider provider, [NotNull]Action<IServiceProvider, IReceivedMessage> onReceived) =>
            Subscribe(queueName, 1, provider, onReceived);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="provider"></param>
        /// <param name="onReceived">回调，消息是否处理完成</param>
        public void Subscribe([NotNull] string queueName, byte concurrencySize, IServiceProvider provider, [NotNull] Action<IServiceProvider, IReceivedMessage> onReceived) =>
            Subscribe(queueName, concurrencySize, true,
                provider ?? throw new ArgumentNullException(nameof(provider))
                , (p, message) =>
                {
                    Semaphore.Release();
                    try
                    {
                        onReceived(p, message);
                    }
                    catch (Exception ex)
                    {
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
        /// <summary>无需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="onReceivedAsync">回调。不管消息处理结果，都算处理完成</param>
        public void SubscribeAsync([NotNull]string queueName, [NotNull]Func<IReceivedMessage, Task> onReceivedAsync) =>
            SubscribeAsync(queueName, 1, onReceivedAsync);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="onReceivedAsync">回调</param>
        public void SubscribeAsync([NotNull]string queueName, byte concurrencySize, [NotNull]Func<IReceivedMessage, Task> onReceivedAsync) =>
            SubscribeAsync(queueName, concurrencySize, true, null, async (_, message) =>
            {
                Semaphore.Release();
                try
                {
                    await onReceivedAsync(message).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex);
                }
                finally
                {
                    if (!Cts.IsCancellationRequested)
                        await Semaphore.WaitAsync().ConfigureAwait(false);
                }
            });

        /// <summary>无需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="provider"></param>
        /// <param name="onReceivedAsync">回调。不管消息处理结果，都算处理完成</param>
        public void SubscribeAsync([NotNull]string queueName, IServiceProvider provider, [NotNull]Func<IServiceProvider, IReceivedMessage, Task> onReceivedAsync) =>
            SubscribeAsync(queueName, 1, provider, onReceivedAsync);

        /// <summary>需回复，异步回调</summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="concurrencySize">并发量</param>
        /// <param name="provider"></param>
        /// <param name="onReceivedAsync">回调</param>
        public void SubscribeAsync([NotNull] string queueName, byte concurrencySize, IServiceProvider provider, [NotNull] Func<IServiceProvider, IReceivedMessage, Task> onReceivedAsync) =>
            SubscribeAsync(queueName, concurrencySize, true,
                provider ?? throw new ArgumentNullException(nameof(provider)),
                async (p, message) =>
                {
                    Semaphore.Release();
                    try
                    {
                        await onReceivedAsync(p, message).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(ex);
                    }
                    finally
                    {
                        if (!Cts.IsCancellationRequested)
                            await Semaphore.WaitAsync().ConfigureAwait(false);
                    }
                }); 
        #endregion
    }
}
