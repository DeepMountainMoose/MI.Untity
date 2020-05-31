using JetBrains.Annotations;
using MI.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.MessageQueue
{
    public class RabbitMQClient:IDisposable
    {
        [CanBeNull] private readonly RabbitMQOptions _options;
        protected readonly ILogger _logger;
        [CanBeNull]
        protected readonly IMeasureMetricsManager _metricsManager;
        protected readonly SuccessfulLazy<IConnection> _rabbitMqConnection;
        //private readonly ApplicationName _name;
        public string applicationName = string.Empty;

        #region ctor
        public RabbitMQClient([NotNull]RabbitMQOptions options, IServiceProvider provider, ILogger logger)
            : this(options.GetFactory(), provider, logger) => _options = options;

        public RabbitMQClient([NotNull]IConnectionFactory factory, IServiceProvider provider, ILogger logger)
        {
            //_name = provider.GetRequiredService<ApplicationName>();
            applicationName = Assembly.GetEntryAssembly().GetName().Name;
            _logger = logger;
            _metricsManager = (IMeasureMetricsManager)provider.GetService(typeof(IMeasureMetricsManager));

            _rabbitMqConnection = new SuccessfulLazy<IConnection>(() =>
            {
                var connection = factory.CreateConnection($"{Environment.MachineName}");

                connection.CallbackException += Connection_CallbackException;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;
                connection.ConnectionUnblocked += Connection_ConnectionUnblocked;
                connection.ConnectionRecoveryError += Connection_ConnectionRecoveryError;
                connection.RecoverySucceeded += Connection_RecoverySucceeded;
                connection.ConnectionShutdown += Connection_ConnectionShutdown;

                return connection;
            });
        }

        #endregion

        #region Connection Event
        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (sender is IConnection connection)
                _logger.LogInformation($"Connection {connection.ClientProvidedName} shutdown,Initiator:{e.Initiator},ReplyText:{e.ReplyText}");
        }

        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            if (sender is IConnection connection)
                _logger.LogInformation($"Connection {connection.ClientProvidedName} recovery succeeded");
        }

        private void Connection_ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            if (sender is IConnection connection)
                _logger.LogError($"Connection {connection.ClientProvidedName} recovery error", e.Exception);
        }

        private void Connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            if (sender is IConnection connection)
                _logger.LogInformation($"Connection {connection.ClientProvidedName} unblocked");
        }

        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (sender is IConnection connection)
                _logger.LogError($"Connection {connection.ClientProvidedName} blocked,reason:{e.Reason}");
        }

        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (sender is IConnection connection && e.Exception != null)
                _logger.LogWarning($"Connection {connection.ClientProvidedName} callback exception", e.Exception);
        }
        #endregion

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
            if (_rabbitMqConnection != null && _rabbitMqConnection.IsValueCreated)
                using (var connection = _rabbitMqConnection.Value)
                    connection.Close();

            _disposed = true;
        }
        #endregion

        /// <summary>创建生产者</summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <returns>生产者</returns>
        public virtual RabbitMQProducer CreateProducer([NotNull]string exchangeName) => CreateProducer(exchangeName, true);

        /// <summary>创建生产者</summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="needConfirm">是否需要确认消息送达</param>
        /// <returns>生产者</returns>
        public virtual RabbitMQProducer CreateProducer([NotNull] string exchangeName, bool needConfirm) =>
            needConfirm
                ? new ConfirmSelectProducer(_rabbitMqConnection.Value, exchangeName, _logger,
                    _metricsManager?.GetMetrics(MetricsDefaults.Architecture), _options?.NotificationProducerCount ?? 10)
                : new RabbitMQProducer(_rabbitMqConnection.Value.CreateModel(), exchangeName);

        #region CreateConsumer
        /// <summary>创建Ack消费者</summary>
        public RabbitMQAckConsumer CreateConsumer() => new RabbitMQAckConsumer(_rabbitMqConnection.Value.CreateModel(), applicationName);

        /// <summary>创建Ack消费者</summary>
        /// <param name="consumerName">消费都名称后缀</param>
        public RabbitMQAckConsumer CreateConsumer([NotNull]string consumerName) => new RabbitMQAckConsumer(_rabbitMqConnection.Value.CreateModel(), applicationName, consumerName);

        /// <summary>创建Noack消费者</summary>
        public RabbitMQNoackConsumer CreateNoackConsumer() => new RabbitMQNoackConsumer(_rabbitMqConnection.Value.CreateModel(), applicationName);

        /// <summary>创建Noack消费者</summary>
        /// <param name="consumerName">消费都名称后缀</param>
        public RabbitMQNoackConsumer CreateNoackConsumer([NotNull]string consumerName) => new RabbitMQNoackConsumer(_rabbitMqConnection.Value.CreateModel(), applicationName, consumerName); 
        #endregion
    }
}
