using MI.Common;
using MI.Library.Integration.AspNetCore.DependencyInjection;
using MI.Library.Integration.AspNetCore.Extensions;
using MI.Library.Integration.AspNetCore.Options;
using MI.Library.Integration.Common.Service;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using FeIModule = MI.Core.Modules.Module;

namespace MI.Library.Integration.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     增加核心服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="platform"></param>
        /// <param name="startupModeType"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiCore(this IServiceCollection services, Platform platform, StartupModeType startupModeType)
        {
            var builder = new LibraryBuilder(services);

            StartupConfig.SetPlatform(platform);
            StartupConfig.SetStartupMode(startupModeType);

            services.AddSingleton<IEnvironmentProvider, DefaultEnvironmentProvider>();

            services.AddOptions();

            services.AddMemoryCache();

            services.AddObjectPool();

            services.AddResponseCompression();

            return builder;
        }


        /// <summary>
        ///     增加全部服务.
        ///     <remarks>
        ///         集成Apollo/Mvc/Swagger/ServiceClient/ApplicationInsights/Authentication/HealthChecks/FeI
        ///     </remarks>
        /// </summary>
        /// <typeparam name="TStartupModule"></typeparam>
        /// <param name="services"></param>
        /// <param name="platform"></param>
        /// <param name="startupModeType"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceProvider AddEhi<TStartupModule>(this IServiceCollection services, Platform platform,
            StartupModeType startupModeType,
            Action<LibraryStartupOptions> optionsAction = null)
            where TStartupModule : FeIModule
        {
            var libraryStartupOptions = new LibraryStartupOptions();
            // 核心服务
            var builder = services.AddEhiCore(platform, startupModeType);

            // Logging
            builder.AddEhiLogging();

            // Apollo
            builder.AddEhiApollo(libraryStartupOptions.ApolloConfigureAction);

            // Mvc
            builder.AddEhiMvc(libraryStartupOptions.MvcOptionsAction);

            // Swagger
            builder.AddEhiSwagger(libraryStartupOptions.SwaggerGenOptionsAction);

            // ServiceClient
            builder.AddEhiServiceClient(libraryStartupOptions.ServiceClientOptionsAction);

            // HealthChecks
            builder.AddEhiHealthChecks(libraryStartupOptions.HealthChecksBuilderAction);

            return builder.AddEhiFeI<TStartupModule>();
        }

        /// <summary>
        /// 日志
        /// </summary>
        public static ILibraryBuilder AddEhiLogging(this ILibraryBuilder builder)
        {
            if (Debugger.IsAttached)
            {
                builder.Services.Configure<LoggerFilterOptions>(options =>
                {
                    options.MinLevel = LogLevel.Debug;
                    options.Rules.Clear();
                });
            }
            else
            {
                builder.Services.Configure<LoggerFilterOptions>(options =>
                {
                    options.MinLevel = LogLevel.Information;
                    options.Rules.Clear();
                });
            }

            builder.Services.AddLogging();
            return builder;
        }

        /// <summary>
        ///     增加对象池
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddObjectPool(this IServiceCollection services)
        {
            services.AddSingleton(new DefaultObjectPoolProvider().CreateStringBuilderPool());

            return services;
        }
    }
}
