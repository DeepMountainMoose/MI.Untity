using MI.Core;
using MI.Core.Modules;
using Microsoft.AspNetCore.Builder;
using System;

namespace MI.Web
{
    /// <summary>
    ///     Owin扩展
    /// </summary>
    public static class OwinExtensions
    {
        /// <summary>
        ///     启用FeI基础结构
        /// </summary>
        /// <typeparam name="TStartupModule">作为FeI启动模块的类型,此类型必须是继承自<see cref="Module"/></typeparam>
        public static IApplicationBuilder UseFeI<TStartupModule>(this IApplicationBuilder app)
            where TStartupModule : Module
        {
            app.UseFeI<TStartupModule>(bootstrapper => { });
            return app;
        }

        /// <summary>
        ///     启用FeI基础结构
        /// </summary>
        /// <typeparam name="TStartupModule">作为FeI启动模块的类型,此类型必须是继承自<see cref="Module"/></typeparam>
        public static IApplicationBuilder UseFeI<TStartupModule>(this IApplicationBuilder appBuilder, Action<Bootstrapper> configureAction) where TStartupModule : Module
        {
            Check.NotNull(appBuilder, nameof(appBuilder));

            if (!appBuilder.Properties.ContainsKey("_FeIBootstrapper.Instance"))
            {
                var bootstrapper = Bootstrapper.Create<TStartupModule>();
                configureAction?.Invoke(bootstrapper);
                bootstrapper.Initialize();
                appBuilder.Properties.Add("_FeIBootstrapper.Instance", bootstrapper);
            }

            return appBuilder;
        }
    }
}
