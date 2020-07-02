using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using MI.Common;
using MI.Library.Integration.Common.Extensions;
using MI.Library.Interface;
using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Integration.Common.Utils
{
    public static class EnvironmentUtils
    {
#if NETFRAMEWORK
        private const string ProductionEnv = "production";
#else
        private const string ProductionEnv = "prod";
#endif

        private const string DemoEnv = "demo";

        public static Env GetApolloEnv(string envStr)
        {
            if (ProductionEnv.Equals(envStr, StringComparison.OrdinalIgnoreCase))
                return Env.Pro;
            else if (DemoEnv.Equals(envStr, StringComparison.OrdinalIgnoreCase))
                return Env.Uat;
            return Env.Dev;
        }

        public static ApolloOptions GetApolloOptions(Platform platform, EnvironmentType environmentType, bool isDebug)
        {
            var options = new ApolloOptions
            {
                AppId = ((int)platform).ToString(),
                Env = environmentType.ToApolloEnv(),
                MetaServer = GetConfigUrl(environmentType),
                HttpMessageHandlerFactory = () => new System.Net.Http.HttpClientHandler { UseProxy = isDebug }
            };

            var cluster = Environment.GetEnvironmentVariable(Constants.Apollo.ClusterEnvironment);
            if (!string.IsNullOrEmpty(cluster))
                options.Cluster = cluster;
            return options;
        }

        public static EnvironmentType GetEnvType(string envStr)
        {
            if (ProductionEnv.Equals(envStr, StringComparison.OrdinalIgnoreCase))
                return EnvironmentType.Product;
            else if (DemoEnv.Equals(envStr, StringComparison.OrdinalIgnoreCase))
                return EnvironmentType.Demo;
            return EnvironmentType.Default;
        }

        public static string GetConfigUrl(EnvironmentType environmentType)
        {
            //switch (environmentType)
            //{
            //    case EnvironmentType.Product:
            //        return "http://apolloConfigEncrypt.1hai.cn";
            //    case EnvironmentType.Demo:
            //        return "http://apollofat.dev.ehi.com.cn:8010";
            //    default:
            //        return "http://apollodev.dev.ehi.com.cn:8010";
            //}

            switch (environmentType)
            {
                case EnvironmentType.Product:
                    return "http://47.99.92.76:8080";
                case EnvironmentType.Demo:
                    return "http://47.99.92.76:8080";
                default:
                    return "http://47.99.92.76:8080";
            }
        }
    }
}
