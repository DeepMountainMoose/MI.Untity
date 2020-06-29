using EnumsNET;
using MI.Core.Configuration.Startup;
using MI.Library.Interface.Common;
using MI.Library.Interface.Configuration;
using MI.Library.Interface.Enum;
using MI.RedisCache.Caching.Redis;
using Microsoft.Extensions.Options;
using System;

namespace MI.Library.Integration.Common.Extensions
{
    public static class StartupConfigurationExtensions
    {
        /// <summary>设置默认数据库配置类型</summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        public static IStartupConfiguration SetExceptionlessKey(this IStartupConfiguration configuration, string key)
        {
            return configuration;
        }

        /// <summary>
        ///     使用Redis缓存
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <param name="option"></param>
        public static void UseRedisCache(this IModuleConfigurations moduleConfigurations, Action<RedisCacheOptions> option = null)
        {
            if (StartupConfig.UseCapability.HasAnyFlags(UseCapability.RedisCache))
                return;
            var config = moduleConfigurations.Configuration.IocManager.Resolve<IOptions<InfrastructureOption>>();
            moduleConfigurations.Configuration.Caching.UseRedis(o =>
            {
                option?.Invoke(o);
                o.ConnectionString = config.Value.RedisCacheConnection;
            });
            StartupConfig.UseCapability = StartupConfig.UseCapability | UseCapability.RedisCache;
        }

        ///// <summary>
        /////     以指定模式使用基于RabbitMq的Rebus
        ///// </summary>
        ///// <param name="moduleConfigurations"></param>
        ///// <param name="queueName">队列名称</param>
        ///// <param name="rebusMode">使用模式</param>
        ///// <param name="numberOfWorkers"></param>
        ///// <param name="maxParallelism"></param>
        ///// <param name="priority">优先级,如果未非null则启用优先级队列</param>
        ///// <param name="retryStrategy">重试次数</param>
        ///// <param name="errorQueue"></param>
        ///// <param name="prefetch"></param>
        //public static void UseRabbitMq(this IModuleConfigurations moduleConfigurations, string queueName, RebusMode rebusMode, int numberOfWorkers = 6, int maxParallelism = 6, int? priority = null, int retryStrategy = 2, string errorQueue = "error", int prefetch = 50)
        //{
        //    var config = moduleConfigurations.Configuration.IocManager.Resolve<IOptions<InfrastructureOption>>();
        //    moduleConfigurations.UseRebus(a =>
        //    {
        //        var builder = a.UseRabbitMq(config.Value.RabbitMqConnection, queueName);
        //        builder.Prefetch(prefetch);
        //        if (priority.HasValue)
        //            builder.PriorityQueue(priority.Value);
        //    })
        //        .Enable(true)
        //        .SetMode(rebusMode)
        //        .SetMaxParallelism(maxParallelism)
        //        .SetNumberOfWorkers(numberOfWorkers)
        //        .UseLogging(a => a.NLog())
        //        .Options(x =>
        //        {
        //            x.Decorate<IPipeline>(res =>
        //            {
        //                var injector = new PipelineStepInjector(res.Get<IPipeline>());
        //                var metricStep = new RebusMonitorStep();
        //                injector.OnReceive(metricStep, PipelineRelativePosition.After,
        //                    typeof(DeserializeIncomingMessageStep));
        //                return injector;
        //            });

        //            x.SimpleRetryStrategy(maxDeliveryAttempts: retryStrategy, errorQueueAddress: errorQueue, secondLevelRetriesEnabled: true);
        //        });

        //    StartupConfig.UseCapability = StartupConfig.UseCapability | UseCapability.RabbitMq;
        //}

        ///// <summary>
        /////     以单向推送队列的形式使用基于RabbitMq的Rebus
        ///// </summary>
        ///// <param name="moduleConfigurations"></param>
        //public static void UseRabbitMqAsOneWayClient(this IModuleConfigurations moduleConfigurations)
        //{
        //    var config = moduleConfigurations.Configuration.IocManager.Resolve<IOptions<InfrastructureOption>>();
        //    moduleConfigurations.UseRebus(a => a.UseRabbitMqAsOneWayClient(config.Value.RabbitMqConnection))
        //            .Enable(true)
        //            .SetMode(RebusMode.Publish)
        //        .Options(x => x.Decorate<IPipeline>(res =>
        //        {
        //            var injector = new PipelineStepInjector(res.Get<IPipeline>());
        //            var metricStep = new RebusMonitorStep();
        //            injector.OnReceive(metricStep, PipelineRelativePosition.After,
        //                typeof(DeserializeIncomingMessageStep));
        //            return injector;
        //        }))
        //        ;

        //    StartupConfig.UseCapability = StartupConfig.UseCapability | UseCapability.RabbitMq;
        //}
    }
}
