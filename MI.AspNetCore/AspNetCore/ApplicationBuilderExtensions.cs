using Castle.LoggingFacility.MsLogging;
using JetBrains.Annotations;
using MI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.AspNetCore.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     使用FeI进行初始化
        /// </summary>
        /// <remarks>此方法应该在Configure里第一个调用</remarks>
        /// <code>
        /// public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        /// {
        /// app.UseFeI();
        /// 
        /// //do something
        /// }
        /// </code>
        /// <param name="app"></param>
        public static void UseFeI(this IApplicationBuilder app)
        {
            app.UseFeI(null);
        }

        /// <summary>
        ///     使用FeI进行初始化
        /// </summary>
        /// <remarks>此方法应该在Configure里第一个调用</remarks>
        /// <code>
        /// public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        /// {
        /// app.UseFeI();
        /// 
        /// //do something
        /// }
        /// </code>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        public static void UseFeI([NotNull] this IApplicationBuilder app, Action<ApplicationBuilderOptions> optionsAction)
        {

            var options = new ApplicationBuilderOptions();
            optionsAction?.Invoke(options);

            if (options.UseCastleLoggerFactory)
            {
                app.UseCastleLoggerFactory();
            }

            InitializeFeI(app);

        }

        private static void InitializeFeI(IApplicationBuilder app)
        {
            var bootstrapper = app.ApplicationServices.GetRequiredService<Bootstrapper>();
            bootstrapper.Initialize();

            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => bootstrapper.Dispose());
        }

        public static void UseCastleLoggerFactory(this IApplicationBuilder app)
        {
            var castleLoggerFactory = app.ApplicationServices.GetService<Castle.Core.Logging.ILoggerFactory>();
            if (castleLoggerFactory == null)
            {
                return;
            }

            app.ApplicationServices
                .GetRequiredService<ILoggerFactory>()
                .AddCastleLogger(castleLoggerFactory);
        }
    }
}
