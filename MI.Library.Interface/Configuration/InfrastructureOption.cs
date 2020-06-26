using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Configuration
{
    public sealed class InfrastructureOption
    {
        /// <summary>
        ///     Redis连接字符串
        /// </summary>
        public string RedisCacheConnection { get; set; }

        /// <summary>
        ///     消息队列
        /// </summary>
        public string RabbitMqConnection { get; set; }

        /// <summary>
        ///     Redis分布式锁
        /// </summary>
        public string RedisDistributelockConnection { get; set; }

        /// <summary>
        ///     Redis Session连接
        /// </summary>
        //public string RedisSessionConnection { get; set; }

        //public string ExceptionlessKey { get; set; }

        //public string ApplicationInsightsKey { get; set; }

        ///// <summary>
        /////     Azure表存储连接
        ///// </summary>
        //public string AzureTableStorageConnection { get; set; }
    }
}
