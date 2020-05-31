using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public static class TuhuBuilderExtensions
    {
        public static ITuhuBuilder AddMessageQueue(this ITuhuBuilder builder) =>
                    builder.AddMessageQueue("rabbitmq");

        public static ITuhuBuilder AddMessageQueue(this ITuhuBuilder builder, string sectionName) =>
            builder.AddMessageQueue(builder.Configuration.GetSection(sectionName));

        public static ITuhuBuilder AddMessageQueue(this ITuhuBuilder builder, [NotNull] IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            builder.Services.TryAddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();
            builder.Services.TryAddSingleton<ITuhuNotification, DefaultTuhuNotification>();
            builder.Services.TryAddSingleton<TuhuMessage>();
            builder.Services.TryAddSingleton<RabbitMQProducerManager>();
            builder.Services.Configure<RabbitMQOptions>(options=>{
                options.HostName = configuration["HostName"];
                options.UserName= configuration["UserName"];
                options.Password = configuration["Password"];
            });

            return builder;
        }

        public static IServiceCollection AddMessageQueue(this IServiceCollection services, [NotNull] IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.TryAddSingleton<IRabbitMQClientFactory, RabbitMQClientFactory>();
            services.TryAddSingleton<ITuhuNotification, DefaultTuhuNotification>();
            services.TryAddSingleton<TuhuMessage>();
            services.TryAddSingleton<IMINotification, MINotification>();
            services.TryAddSingleton<RabbitMQProducerManager>();
            services.Configure<RabbitMQOptions>(options => {
                options.HostName = configuration["rabbitmq:HostName"];
                options.UserName = configuration["rabbitmq:UserName"];
                options.Password = configuration["rabbitmq:Password"];
            });

            return services;
        }
    }
}
