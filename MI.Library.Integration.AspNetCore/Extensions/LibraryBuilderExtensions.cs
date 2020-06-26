using Com.Ctrip.Framework.Apollo;
using EnumsNET;
using MI.AspNetCore;
using MI.Core;
using MI.Library.Enum;
using MI.Library.Exceptions;
using MI.Library.Handler;
using MI.Library.Integration.AspNetCore.DependencyInjection;
using MI.Library.Integration.AspNetCore.Filter.swagger;
using MI.Library.Integration.AspNetCore.Options;
using MI.Library.Integration.AspNetCore.Service;
using MI.Library.Integration.Common.Utils;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using MI.Library.Interface.Configuration;
using MI.Library.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using FeIModule = MI.Core.Modules.Module;

namespace MI.Library.Integration.AspNetCore.Extensions
{
    public static class LibraryBuilderExtensions
    {
        /// <summary>
        ///     增加FeI
        /// </summary>
        /// <typeparam name="TStartupModule"></typeparam>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceProvider AddEhiFeI<TStartupModule>(this ILibraryBuilder builder,
            Action<BootstrapperOptions> optionsAction = null)
            where TStartupModule : FeIModule
        {
            return builder.Services.AddFeI<TStartupModule>(optionsAction);
        }

        /// <summary>
        ///     增加Mvc
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiMvc(this ILibraryBuilder builder, Action<MvcOptions> optionsAction = null)
        {
            if (optionsAction == null) optionsAction = options => { options.InitLibraryOption(); };

            builder.Services.AddMvc(a =>
            {
                //if (StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Ui))
                //{
                //    var jsonFormatter =
                //        (JsonOutputFormatter)a.OutputFormatters.FirstOrDefault(f => f is JsonOutputFormatter);
                //    if (jsonFormatter != null)
                //        jsonFormatter.PublicSerializerSettings.NullValueHandling = NullValueHandling.Include;
                //}

                optionsAction?.Invoke(a);
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            return builder;
        }

        /// <summary>
        ///     增加Apollo
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiApollo(this ILibraryBuilder builder,
            Action<IServiceCollection, IConfiguration> configureAction = null)
        {
            var isDebug = Debugger.IsAttached;

            var environmentProvider =
                builder.Services.BuildServiceProvider().GetService<IEnvironmentProvider>();

            if (environmentProvider == null)
                throw new LibraryException("没有环境提供程序");

            var platForm = StartupConfig.CurrentPlatform;
            var environmentType = environmentProvider.GetCurrentEnvironment();

            var configurationRoot = new ConfigurationBuilder()
                .AddApollo(EnvironmentUtils.GetApolloOptions(platForm, environmentType, isDebug))
                .AddNamespace("EHI.Front")
                .AddDefault()
                .Build();

            foreach (var item in configurationRoot.AsEnumerable())
                configurationRoot[item.Key] = AesUtils.Descrypt(item.Value, Constants.Apollo.Aes.Key, Constants.Apollo.Aes.Iv);

            builder.Services.Configure<InfrastructureOption>(configurationRoot);
            configureAction?.Invoke(builder.Services, configurationRoot);

            return builder;
        }

        /// <summary>
        ///     增加Swagger
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiSwagger(this ILibraryBuilder builder,
            Action<SwaggerGenOptions> optionsAction = null)
        {
            if (optionsAction == null)
                optionsAction = c => {
                    if (StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Admin))
                    {
                        c.AddSecurityDefinition("bearerAuth", new ApiKeyScheme
                        {
                            Description =
                                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = "header",
                            Type = "apiKey"
                        });
                        c.OperationFilter<SecurityRequirementsOperationFilter>();
                    }

                    c.OperationFilter<AdditionOperationFilter>();

                    var currentAssembly = Assembly.GetEntryAssembly();
                    if (currentAssembly == null)
                        return;

                    c.SwaggerDoc(currentAssembly.GetName().Name, new Info
                    {
                        Version = "v1",
                        Title = currentAssembly.GetName().Name
                    });
                    c.CustomSchemaIds(t => t.FullName);

                    foreach (var xmlFile in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "eHi.*.xml"))
                    {
                        c.IncludeXmlComments(xmlFile, true);
                    }
                };

            builder.Services.AddSwaggerGen(optionsAction);

            return builder;
        }

        /// <summary>
        ///     增加ServiceClient
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiServiceClient(this ILibraryBuilder builder,
            Action<ServiceClientOptions> optionsAction = null)
        {
            var options = new ServiceClientOptions();

            optionsAction?.Invoke(options);

            var isUseProxy = options?.IsUseProxy() ?? false;

            builder.Services.AddHttpClient(nameof(IResilientServiceClient))
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    UseProxy = isUseProxy
                })
                .AddHttpMessageHandler(s => new RequestContextHandler(
                    StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Ui) ?
                    PlatformPriority.SetDataPlatform : PlatformPriority.CurrentPlatform
                    , StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Ui)))
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(options.ResilientHttpRetryCount, _ => TimeSpan.FromMilliseconds(10)))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(options.ResilientHttpTimeout));

            builder.Services.AddSingleton<IResilientServiceClient, ResilientServiceClient>();

            builder.Services.AddHttpClient("HealthCheck")
                .AddPolicyHandler(HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(options.ResilientHttpRetryCount, _ => TimeSpan.FromMilliseconds(10)))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(options.ResilientHttpTimeout));

            return builder;
        }

        /// <summary>
        ///     增加HealthChecks
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static ILibraryBuilder AddEhiHealthChecks(this ILibraryBuilder builder,
            Action<IHealthChecksBuilder> builderAction = null)
        {
            var healthChecksBuilder = builder.Services.AddHealthChecks();

            builderAction?.Invoke(healthChecksBuilder);

            return builder;
        }
    }
}
