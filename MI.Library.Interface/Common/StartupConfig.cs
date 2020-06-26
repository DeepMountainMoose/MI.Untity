using MI.Common;
using MI.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Common
{
    public static class StartupConfig
    {
        private static string _cachePrefix = string.Empty;
        /// <summary>
        ///     缓存环境前缀
        /// </summary>
        public static string CacheEnvironmentPrefix { get; set; }
        public static Platform CurrentPlatform { get; set; } = Platform.None;
        public static StartupModeType StartupMode { get; private set; }

        /// <summary>
        ///     缓存Key前缀
        /// </summary>
        public static string CachePrefix
        {
            get => CacheEnvironmentPrefix + _cachePrefix;
            internal set => _cachePrefix = value.EndsWith(":") ? value : value + ":";
        }

        public static void PostInitialize()
        {
            if (_cachePrefix.IsNullOrEmpty())
                _cachePrefix = ((int)CurrentPlatform % 1010000) + ":";
        }

        public static void SetPlatform(Platform platform)
        {
            CurrentPlatform = platform;
        }

        public static void SetStartupMode(StartupModeType startupMode)
        {
            StartupMode = startupMode;
        }
    }
}
