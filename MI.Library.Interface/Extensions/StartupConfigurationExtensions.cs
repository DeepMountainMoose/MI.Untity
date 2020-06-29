using MI.Common;
using MI.Core.Configuration.Startup;
using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Extensions
{
    /// <summary>平台扩展</summary>
    public static class StartupConfigurationExtensions
    {
        /// <summary>设置当前平台</summary>
        /// <param name="configuration"></param>
        /// <param name="platform"></param>
        public static IStartupConfiguration SetPlatform(this IStartupConfiguration configuration, Platform platform)
        {
            SetPlatform(platform);
            return configuration;
        }

        /// <summary>设置缓存Key前缀</summary>
        /// <param name="configuration"></param>
        /// <param name="cachePrefix"></param>
        public static IStartupConfiguration SetCachePrefix(this IStartupConfiguration configuration, string cachePrefix)
        {
            StartupConfig.CachePrefix = cachePrefix;
            return configuration;
        }


        /// <summary>
        ///     设置当前平台
        /// </summary>
        /// <param name="platform"></param>
        public static void SetPlatform(Platform platform)
        {
            StartupConfig.CurrentPlatform = platform;
        }

        /// <summary>
        ///     设置默认数据库配置类型
        /// </summary>
        /// <param name="dbConfigType"></param>
        public static void SetDbConfigType(DbConfigType dbConfigType)
        {
            StartupConfig.DefaultDbConfigType = dbConfigType;
        }
    }
}
