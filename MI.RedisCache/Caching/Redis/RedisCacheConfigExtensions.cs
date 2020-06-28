using MI.Core.Extensions;
using MI.Core.Runtime.Caching;
using MI.Core.Runtime.Caching.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    public static class RedisCacheConfigExtensions
    {
        /// <summary>
        ///     Configures caching to use Redis as cache server.
        ///     <para>配置为使用Redis缓存服务器</para>
        /// </summary>
        /// <param name="cachingConfiguration">
        ///     The caching configuration.
        ///     <para>缓存配置</para>
        /// </param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.Configuration.IocManager.RegisterTypeIfNot<ICacheManager, RedisCacheManager>();
        }

        /// <summary>
        ///     Configures caching to use Redis as cache server.
        ///     <para>配置为使用Redis缓存服务器</para>
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<RedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.Configuration.IocManager;

            iocManager.RegisterTypeIfNot<ICacheManager, RedisCacheManager>();

            optionsAction(iocManager.Resolve<RedisCacheOptions>());
        }
    }
}
