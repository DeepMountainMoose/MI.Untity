using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Enum
{
    /// <summary>
    ///     已使用的Library的功能集
    /// </summary>
    [Flags]
    public enum UseCapability
    {
        None,
        /// <summary>
        ///     Redis缓存
        /// </summary>
        RedisCache = 1,
        /// <summary>
        ///     Redis分布式锁
        /// </summary>
        RedisLock = 2,
        /// <summary>
        ///     RabbitMq队列
        /// </summary>
        RabbitMq = 4
    }
}
