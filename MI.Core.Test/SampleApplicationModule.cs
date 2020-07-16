using MI.Common;
using MI.Core.Extensions;
using MI.Core.Modules;
using MI.Library.Enum;
using MI.Library.Extensions;
using MI.Library.Handler;
using ServiceClient;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using MI.Library.Integration.Common;
using MI.Test.Repositories;

namespace MI.Core.Test
{
    [DependsOn(typeof(KernelModule), typeof(IntegrationCommonModule), typeof(RepositiesModule))]
    public class SampleApplicationModule : Modules.Module
    {
        /// <summary>
        /// 预初始化
        /// </summary>
        public override void PreInitialize()
        {
            ConfigServiceClient();
            ThreadPool.SetMinThreads(Math.Max(50, Environment.ProcessorCount * 5), Math.Min(80, Environment.ProcessorCount * 10));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Configuration.SetPlatform(Platform.TestApi)
                         .SetCachePrefix("fSkockApi");
        }

        private void ConfigServiceClient()
        {
            if (!IocManager.IsRegistered<IServiceClient>())
            {
                var serviceClient =
                    new ServiceClient.ServiceClient(new RequestContextHandler(PlatformPriority.CurrentPlatform, false)
                    {
                        InnerHandler = new HttpClientHandler
                        { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate },
                    });
                IocManager.RegisterInstance<IServiceClient>(serviceClient);
            }
        }
    }
}
