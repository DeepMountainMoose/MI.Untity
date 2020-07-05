using Com.Ctrip.Framework.Apollo;
using MI.Common;
using MI.Library.Integration.Common.Extensions;
using MI.Library.Integration.Common.Utils;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using MI.Library.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MI.Library.Integration.Common.Service
{
    /// <summary>
    ///     适配阿波罗的数据库连接字符串获取服务
    /// <remarks>本类Aegis和FeI同时复用所有会有多个编译指令</remarks>
    /// </summary>
    public sealed class ApolloDbConnectionStringResolver : IDbConnectionStringResolver
    {
        #region [ Filed ]

        private readonly object _syncRoot = new object();
        private IConfigurationRoot _configuration;

        private readonly Dictionary<string, string> _cacheDic;

        private readonly IReadOnlyDictionary<int, string> _dbConfigKeyMap;

        #endregion

        #region [ Init ]
#if AEGIS
        public ApolloDbConnectionStringResolver(IHostEnvironment environment, IOptions<LibraryOptions> options)
        {
            _cacheDic = new Dictionary<string, string>();
            _dbConfigKeyMap = GetTeamMap(options.Value.Platform.ToTeamType());

            Init(environment.EnvironmentName);
        }

        private void Init(string environment)
        {
            var env = ApolloHelper.ToApolloEnv(environment);
            var metaServer = ApolloHelper.GetApolloMetaServer(env);

            var apolloOptions = ApolloHelper.BuildApolloOptions(Constants.Apollo.Database.AppId, env, metaServer, false);

            var cluster = Environment.GetEnvironmentVariable(Constants.Apollo.ClusterEnvironment);

            if (!string.IsNullOrEmpty(cluster))
            {
                apolloOptions.Cluster = cluster;
            }

            var configuration = new ConfigurationBuilder()
                .AddApollo(apolloOptions)
                .AddNamespace(Constants.Apollo.Database.Namespace)
                .Build();

            _configuration = configuration;
        }
#else
        public ApolloDbConnectionStringResolver(IEnvironmentProvider environmentProvider)
        {
            _cacheDic = new Dictionary<string, string>();
            _dbConfigKeyMap = GetTeamMap(StartupConfig.CurrentPlatform.ToTeamType());
            Init(environmentProvider.GetCurrentEnvironment());
            environmentProvider.OnEnvironmentChanged += EnvironmentProvider_OnEnvironmentChanged;
        }

        private void EnvironmentProvider_OnEnvironmentChanged(object sender, EventArgs.EnvironmentChangedEventArgs e)
        {
            Init(e.NewEnvironment);
        }

        private void Init(EnvironmentType environmentType)
        {
            _configuration = new ConfigurationBuilder()
                .AddApollo(new ApolloOptions
                {
                    AppId = Constants.Apollo.Database.AppId,
                    Env = environmentType.ToApolloEnv(),
                    MetaServer = EnvironmentUtils.GetConfigUrl(environmentType),
                    HttpMessageHandlerFactory = () => new System.Net.Http.HttpClientHandler { UseProxy = false }
                }).AddNamespace(Constants.Apollo.Database.Namespace).AddDefault().Build();
        }
#endif
        #endregion

        /// <inheritdoc />
        public string ResolveConnectionString(DbConfigType configType)
        {
            if (!_cacheDic.TryGetValue(_configuration[_dbConfigKeyMap[(int)configType]],
                out var connectionString))
            {
                lock (_syncRoot)
                {
                    if (!_cacheDic.TryGetValue(_configuration[_dbConfigKeyMap[(int)configType]],
                        out connectionString))
                    {
                        //connectionString = AesUtils.Descrypt(_configuration[_dbConfigKeyMap[(int)configType]],
                        //    Constants.Apollo.Aes.Key, Constants.Apollo.Aes.Iv);

                        connectionString = _configuration[_dbConfigKeyMap[(int)configType]];
                        _cacheDic.Add(_configuration[_dbConfigKeyMap[(int)configType]], connectionString);
                    }

                }
            }

            return connectionString;
        }

        /// <inheritdoc />
        public string ResolveConnectionString(DbConfigType configType, string parameter1)
        {
            if (!_cacheDic.TryGetValue(_configuration[string.Format(_dbConfigKeyMap[(int)configType], parameter1)],
                out var connectionString))
            {
                lock (_syncRoot)
                {
                    if (!_cacheDic.TryGetValue(
                        _configuration[string.Format(_dbConfigKeyMap[(int)configType], parameter1)],
                        out connectionString))
                    {

                        ////AES解密
                        //connectionString = AesUtils.Descrypt(_configuration[string.Format(_dbConfigKeyMap[(int)configType], parameter1)],
                        //    Constants.Apollo.Aes.Key, Constants.Apollo.Aes.Iv);

                        connectionString = _configuration[string.Format(_dbConfigKeyMap[(int)configType], parameter1)];
                        _cacheDic.Add(_configuration[string.Format(_dbConfigKeyMap[(int)configType], parameter1)], connectionString);
                    }

                }
            }
            return connectionString;

        }

        /// <summary>
        /// Apollo数据库连接字符串
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        private static IReadOnlyDictionary<int, string> GetTeamMap(TeamType team)
        {
            switch (team)
            {
                case TeamType.MI:
                    return new Dictionary<int, string>
                    {
                        {(int)DbConfigType.Default,"Apollo_GW_MI"},
                        {(int)DbConfigType.MI,"Apollo_GW_MI"},
                    };
                case TeamType.Callctr:
                    return new Dictionary<int, string>
                    {
                        {(int)DbConfigType.Default,"Apollo_GW_MI"},
                        {(int)DbConfigType.MI,"Apollo_GW_MI"},
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, "请参考");
            }
        }
    }
}
