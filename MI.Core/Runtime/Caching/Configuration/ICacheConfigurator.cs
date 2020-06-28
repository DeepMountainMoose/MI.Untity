using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime.Caching.Configuration
{
    /// <summary>
    ///     缓存配置器.
    /// </summary>
    public interface ICacheConfigurator
    {
        /// <summary>
        ///     缓存的名字.
        ///     如果此值为null则为配置所有缓存
        /// </summary>
        string CacheName { get; }

        /// <summary>
        ///     缓存配置行为.当缓存创建后执行一些初始化操作的委托
        /// </summary>
        Action<ICache> InitAction { get; }
    }
}
