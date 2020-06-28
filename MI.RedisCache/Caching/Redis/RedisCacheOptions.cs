using MI.Core.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    public class RedisCacheOptions
    {
        public IStartupConfiguration StartupConfiguration { get; }

        private const string ConnectionStringKey = "FeI.Redis.Cache";
        private const string DatabaseIdSettingKey = "FeI.Redis.Cache.DatabaseId";

        public string ConnectionString { get; set; }
        public int DatabaseId { get; set; }

        public RedisCacheOptions(IStartupConfiguration startupConfiguration)
        {
            StartupConfiguration = startupConfiguration;
            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
        }

        private static int GetDefaultDatabaseId()
        {
#if !NETSTANDARD
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
#else
            return -1;
#endif
        }

        private static string GetDefaultConnectionString()
        {
#if !NETSTANDARD
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
#else
            return "localhost";
#endif
        }
    }
}
