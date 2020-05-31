using System;
using System.Collections.Generic;
using System.Text;

namespace MI.MessageQueue
{
    public interface IRabbitMQClientFactory
    {
        /// <summary>获取默认连接，请别dispose了</summary>
        RabbitMQClient GetDefaultClient();
        /// <summary>创建新连接，请记得dispose。创建默认配置的连接请使用null或""</summary>
        RabbitMQClient CreateClient(string name);
    }
}
