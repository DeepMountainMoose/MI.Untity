using Castle.MicroKernel.Registration;
using DapperWrapper;
using FeI.RedisCache.ProtoBuf.Caching.Redis;
using MI.Core.Dependency;
using MI.Core.Extensions;
using MI.Core.Modules;
using MI.Library.Integration.Common.Extensions;
using MI.Library.Integration.Common.Service;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using MI.Library.Interface.Extensions;
using MI.Library.Service;
using MI.RedisCache.Caching.Redis;

namespace MI.Library.Integration.Common
{
    [DependsOn(typeof(RedisCacheModule))]
    public class IntegrationCommonModule: Module
    {
        public override void PreInitialize()
        {
            ConfigApplicationUrls();
            Configuration.Modules.UseRedisCache(option => option.UseProtoBuf());
        }

        public override void Initialize()
        {
            if (!IocManager.IsRegistered<IDbConnectionStringResolver>())
            {
                IocManager.RegisterTypeIfNot<IDbConnectionStringResolver, ApolloDbConnectionStringResolver>();
            }
        }

        public override void PostInitialize()
        {
            if (!IocManager.IsRegistered<IDbExecutorFactory>())
                IocManager.GetContainer()
                    .Kernel.Register(
                        Component.For<IDbExecutorFactory, IDbExecutorFactoryWithDbConfigType>()
                            .ImplementedBy<SqlExecutorFactoryWithDbConfigType>());
            DapperWrapperExtensions.SetDbConnectionStringResolver(IocManager.Resolve<IDbConnectionStringResolver>());
        }

        /// <summary>
        ///     <see cref="IApplicationUrlProvider"/>的配置
        /// </summary>
        private void ConfigApplicationUrls()
        {
            IocManager.RegisterType<IApplicationUrlProvider, DefaultApplicationUrlProvider>();
            if (ApplicationUrls.Provider == null)
                ApplicationUrls.Provider = IocManager.Resolve<IApplicationUrlProvider>();
        }
    }
}
