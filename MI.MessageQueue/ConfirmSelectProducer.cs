using JetBrains.Annotations;
using MI.Metrics;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.MessageQueue
{
    /// <summary>消息发送者</summary>
    public class ConfirmSelectProducer:RabbitMQProducer
    {
        private static TimeSpan MaxWaitingTime = TimeSpan.FromSeconds(2);
        private readonly ILogger _logger;
        private readonly AsyncRetryPolicy<bool> _asyncPolicy;
        private readonly Producer _producer;
        [CanBeNull] private readonly IMeasureMetrics _metrics;

        protected internal ConfirmSelectProducer(IConnection connection, string exchangeName, ILogger logger, IMeasureMetrics metrics, int count)
            : base(CreateProducer(connection, exchangeName, Math.Max(1, count), logger, out var producer), exchangeName)
        {
            _logger = logger;
            _metrics = metrics;

            _asyncPolicy = Policy<bool>
                .Handle<OperationInterruptedException>()
                .WaitAndRetryAsync(Retires, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, Retires - retryAttempt - 6)));

            _producer = producer;
        }

        public static IModel CreateProducer(IConnection connection,string exchangeName,int max, ILogger logger, out Producer producer)
        {
            producer = new Producer(connection, exchangeName, max, logger);

            return producer.Channel;
        }

        private async Task Metrics(Func<CancellationToken,Task<bool>> func,string routingKey,CancellationToken cancellationToken)
        {
            try
            {
                var sw = ValueStopwatch.StartNew();
                using (var cts1 = new CancellationTokenSource(MaxWaitingTime))
                using (var cts2 = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cancellationToken))
                    if (!await _asyncPolicy.ExecuteAsync(token => func(token), cts2.Token).ConfigureAwait(false))
                    {
                        Increment("MQ_Send_Nacks", routingKey);

                        throw new IOException("Nacks Received，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#IOException");
                    }
                Increment("MQ_Send", routingKey);
                LogElapsedTime(sw.GetElapsedTime(), routingKey);
            }
            catch (OperationCanceledException ex)
            {
                Increment("MQ_Send_Timeout", routingKey);

                throw new IOException("Timed out waiting for acks，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#IOException", ex);
            }
            catch (TimeoutException)
            {
                Increment("MQ_Send_WaitTimeout", routingKey);

                throw new TimeoutException("串行等待超时，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#TimeoutException");
            }
        }

        private void LogElapsedTime(TimeSpan span, string routingKey)
        {
            if (span.TotalSeconds > 5)
            {
                Increment("MQ_Send_Time5", routingKey);

                _logger.LogError("等待和发送已经超过5秒，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#TimeoutException");
            }
            else if (span.TotalSeconds > 3)
            {
                Increment("MQ_Send_Time3", routingKey);

                _logger.LogWarning("等待和发送已经超过3秒，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#TimeoutException");
            }
            else if (span.TotalSeconds > 1)
            {
                Increment("MQ_Send_Time", routingKey);

                _logger.LogInformation($"等待和发送已经超过1秒，https://gitlab.tuhu.cn/arch_public/DotNet_SDK/blob/master/src/Tuhu.MessageQueue/README.md#TimeoutException");
            }
        }

        private void Increment(string statName, string routingKey) =>
            _metrics?.Counter(statName, tags: new Dictionary<string, string> { { "ExchangeName", ExchangeName }, { "RoutingKey", routingKey } });

        /// <summary>发送消息</summary>
        protected override Task SendAsync(string routingKey, object message, IBasicProperties basicProperties, CancellationToken cancellationToken) =>
            Metrics(token => _producer.Send(routingKey, DataFormatter.Serialize(message), basicProperties, token), routingKey, cancellationToken);

        public override void Send(IReadOnlyCollection<Message> messages) => SendAsync(messages, CancellationToken.None).GetAwaiter().GetResult();

        public override Task SendAsync(IReadOnlyCollection<Message> messages, CancellationToken cancellationToken)
        {
            if (messages.Count < 1) return Task.CompletedTask;

            return Metrics(token => _producer.Send(messages.Select(m => (m.To, DataFormatter.Serialize(m.Body), CreateBasicProperties(m))), token), messages.First().To, cancellationToken);
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;

            if (disposing) _producer.Dispose();

            base.Dispose(disposing);
        }
    }
}
