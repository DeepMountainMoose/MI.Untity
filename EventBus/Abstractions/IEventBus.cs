using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        /// <summary>
        /// 发布
        /// </summary>
        void Publish(string RoutingKey, object Model);

        /// <summary>
        /// 订阅
        /// </summary>
        void Subscribe(string QueueName, string RoutingKey,string ExchangeName="");

        /// <summary>
        /// 取消订阅
        /// </summary>
        void UnSubscribe(string QueueName, string RoutingKey);
    }
}
