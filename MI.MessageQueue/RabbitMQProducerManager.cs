using JetBrains.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class RabbitMQProducerManager:IDisposable
    {
        private readonly ConcurrentDictionary<string, RabbitMQProducer> _producerMap = new ConcurrentDictionary<string, RabbitMQProducer>();
        private readonly ConcurrentDictionary<string, Action<RabbitMQProducer>> _exchangeInitialize = new ConcurrentDictionary<string, Action<RabbitMQProducer>>();
        private readonly RabbitMQClient _client;

        public RabbitMQProducerManager(IRabbitMQClientFactory factory) : this(
            factory.GetDefaultClient()) { }

        public RabbitMQProducerManager(RabbitMQClient client) => _client = client;

        public RabbitMQProducerManager AddExchangeInitialize(string exchangeName, Action<RabbitMQChannel> initialize)
        {
            _exchangeInitialize.TryAdd(exchangeName, initialize);

            return this;
        }

        /// <summary>获取生产者</summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="needConfirm">是否需要确认消息送达</param>
        /// <returns>生产者</returns>
        public IMessageProducer GetProducer([NotNull] string exchangeName, bool needConfirm = true) =>
            _producerMap.GetOrAdd(exchangeName, exchange =>
            {
                RabbitMQProducer producer = null;

                try
                {
                    producer = _client.CreateProducer(exchange, needConfirm);

                    if (_exchangeInitialize.TryGetValue(exchangeName, out var action))
                        action(producer);
                }
                catch
                {
                    producer?.Dispose();

                    throw;
                }

                return producer;
            });

        public void Dispose()
        {
            foreach (var kv in _producerMap)
            {
                try
                {
                    kv.Value.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
