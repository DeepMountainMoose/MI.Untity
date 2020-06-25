using MI.Library.Integration.AspNetCore.DependencyInjection;
using MI.Library.Integration.AspNetCore.Extensions;
using MI.Library.Integration.AspNetCore.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        public static ILibraryBuilder AddEhiCore(this IServiceCollection services)
        {
            var builder = new LibraryBuilder(services);
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
        public static IServiceProvider AddEhi<TStartupModule>(this IServiceCollection services)
            where TStartupModule : FeIModule
        {
            var libraryStartupOptions = new LibraryStartupOptions();
            // 核心服务
            var builder = services.AddEhiCore();
            // Mvc
            builder.AddEhiMvc(libraryStartupOptions.MvcOptionsAction);
            return builder.AddEhiFeI<TStartupModule>();
        }
    }
}
