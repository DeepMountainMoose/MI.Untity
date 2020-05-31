using Autofac;
using EventBus.Abstractions;
using EventBus.Events;
using MI.APIClientService;
using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using MI.Untity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "mi_event_bus";
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly ILifetimeScope _autofac;
        private readonly IApiHelperService _apiHelperService;
        private readonly string AUTOFAC_SCOPE_NAME = "mi_event_bus";
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;


        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac, IApiHelperService apiHelperService, string queueName = null, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueName = queueName;
            //_consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _retryCount = retryCount;
            _apiHelperService = apiHelperService;
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        public void Publish(string routingKey, object Model)
        {
            _logger.LogInformation($"MQ准备发布消息 RoutingKey:{routingKey} Message:{JsonConvert.SerializeObject(Model)}");

            //如果RabiitMQ没有连接，则创建连接
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            //通过Polly定义重试策略 用于RabbitMQ发布消息
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    //记录错误信息
                    _logger.LogWarning(ex.ToString());
                });

            //创建RabbitMQ信道
            using (var channel = _persistentConnection.CreateModel())
            {
                //声明交换器 模式为 direct
                channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
                //消息序列化
                var message = JsonConvert.SerializeObject(Model);
                //转化为二进制
                var body = Encoding.UTF8.GetBytes(message);

                //执行Polly的重试策略
                policy.Execute(() =>
                {
                    //设置消息持久化
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; //持久化

                    //发布RabbitMQ消息
                    channel.BasicPublish(exchange: BROKER_NAME, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body);
                });

                //记录消息发布日志 用于后期查询问题原因（非常重要）
                _logger.LogInformation($"MQ成功发布消息 RoutingKey:{routingKey} Message:{JsonConvert.SerializeObject(Model)}");
            }
        }

        /// <summary>
        /// RabbitMQ订阅方法
        /// </summary>
        /// <param name="QueueName">队列名</param>
        /// <param name="RoutingKey">路由键</param>
        /// <param name="ExchangeName">交换器名</param>
        public void Subscribe(string QueueName, string RoutingKey, string ExchangeName)
        {
            //判断RabbitMQ是否正常连接  否则创建连接
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            ExchangeName = string.IsNullOrWhiteSpace(ExchangeName) ? BROKER_NAME : ExchangeName;

            //创建信道
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);
            }
        }

        public void UnSubscribe(string QueueName, string RoutingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.QueueUnbind(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: RoutingKey);
            }
        }


        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                _logger.LogInformation($"MQ准备消费消息 RoutingKey:{ea.RoutingKey} Message:{message}");

                await ProcessEvent(ea.RoutingKey, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
                _logger.LogInformation($"MQ成功确认消费消息 RoutingKey:{ea.RoutingKey} Message:{message}");
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        /// <summary>
        /// 发送MQ到指定服务接口
        /// </summary>
        private async Task ProcessEvent(string routingKey, string message)
        {
            try
            {
                _logger.LogInformation($"MQ准备执行ProcessEvent方法，RoutingKey:{routingKey} Message:{message}");
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    //获取绑定该routingKey的服务地址集合
                    var subscriptions = await StackRedis.Current.GetAllList(routingKey);
                    if (!subscriptions.Any())
                    {
                        //如果Redis中不存在 则从数据库中查询 加入Redis中
                        var response = _apiHelperService.PostAsync<QueryRoutingKeyApiUrlResponse>(ServiceAddress.QueryRoutingKeyApiUrlAsync, new QueryRoutingKeyApiUrlRequest { RoutingKey = routingKey });
                        if (response.Result != null && response.Result.ApiUrlList.Any())
                        {
                            subscriptions = response.Result.ApiUrlList;
                            Task.Run(() =>
                            {
                                StackRedis.Current.SetLists(routingKey, response.Result.ApiUrlList);
                            });

                        }
                    }
                    foreach (var apiUrl in subscriptions)
                    {
                        Task.Run(() =>
                        {
                            _logger.LogInformation(message);
                        });

                        await _apiHelperService.PostAsync(apiUrl, message);
                        _logger.LogInformation($"MQ执行ProcessEvent方法完成，RoutingKey:{routingKey} Message:{message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQ消费失败 RoutingKey:{routingKey} Message:{message}");
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
        }
    }
}
