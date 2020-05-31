using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public class RabbitMQClientFactory : IRabbitMQClientFactory, IDisposable
    {
        private readonly IOptionsMonitor<RabbitMQOptions> _options;
        private readonly ILogger<RabbitMQClientFactory> _logger;
        private readonly IOptionsMonitor<ConnectionFactory> _factory;
        private readonly IServiceProvider _provider;
        private readonly RabbitMQClient _defaultClient;

        public RabbitMQClientFactory(IOptionsMonitor<RabbitMQOptions> options, ILogger<RabbitMQClientFactory> logger, IOptionsMonitor<ConnectionFactory> factory, IServiceProvider provider)
        {
            _options = options;
            _logger = logger;
            _factory = factory;
            _provider = provider;
            _defaultClient = new RabbitMQClient(GetFactory(null), provider, _logger);
        }

        public RabbitMQClient GetDefaultClient() => _defaultClient;

        private IConnectionFactory GetFactory(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var value = _factory.Get(name);
                if (value.HostName != "localhost" && value.UserName != "guest")
                    return value;

                var options = _options.Get(name);
                if (options.HostName != null)
                    return options.GetFactory();
            }

            var currentValue = _factory.CurrentValue;

            return currentValue.HostName != "localhost" && currentValue.UserName != "guest"
                ? currentValue
                : _options.CurrentValue.GetFactory();
        }

        public RabbitMQClient CreateClient(string name) => new RabbitMQClient(GetFactory(name), _provider, _logger);

        public void Dispose() => _defaultClient.Dispose();
    }
}
