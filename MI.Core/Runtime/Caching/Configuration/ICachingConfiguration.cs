using MI.Core.Configuration.Startup;
using System;
using System.Collections.Generic;

namespace MI.Core.Runtime.Caching.Configuration
{
    /// <summary>
    ///     缓存配置.
    /// </summary>
    public interface ICachingConfiguration
    {
        /// <summary>
        ///     获取启动配置对象
        /// </summary>
        IStartupConfiguration Configuration { get; }

        /// <summary>
        ///     所有已注册的缓存配置器集合.
        /// </summary>
        IReadOnlyList<ICacheConfigurator> Configurators { get; }

        /// <summary>
        ///     配置所有缓存.
        /// </summary>
        /// <param name="initAction">
        ///     一个配置缓存的行为
        /// </param>
        void ConfigureAll(Action<ICache> initAction);

        /// <summary>
        ///     配置特定缓存.
        /// </summary>
        /// <param name="cacheName">缓存名称</param>
        /// <param name="initAction">
        ///     一个配置缓存的行为.
        /// </param>
        void Configure(string cacheName, Action<ICache> initAction);
    }
}
