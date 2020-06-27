using EnumsNET;
using HealthChecks.UI.Client;
using MI.AspNetCore.AspNetCore;
using MI.Common;
using MI.Core.Logging;
using MI.Library.Common;
using MI.Library.Integration.AspNetCore.Middleware;
using MI.Library.Integration.AspNetCore.Options;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace MI.Library.Integration.AspNetCore.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        ///     使用请求日志记录
        /// </summary>
        /// <param name="app">builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiHttpRequestLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpRequestLogMiddleware>();
        }

        /// <summary>
        ///     Swagger增加安全登录验证
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiSwaggerAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SwaggerAuthMiddleware>();
        }

        /// <summary>
        ///     增加统一异常处理
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(x =>
            {
                x.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        LogHelper.LogException(ex.Error);
                    }
                    await context.Response.WriteAsync(
                        JsonConvert.SerializeObject(new ApiResult((int)ApiMsgPropmt.Exception, Const.系统错误话术) { IsSuccess = false }, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                    );
                });
            });

            return app;
        }

        /// <summary>
        ///     使用Mvc
        /// </summary>
        /// <param name="app"></param>
        /// <param name="routeBuilderAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiMvc(this IApplicationBuilder app,
            Action<IRouteBuilder> routeBuilderAction = null)
        {
            if (routeBuilderAction == null)
                routeBuilderAction = routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                };

            app.UseMvc(routeBuilderAction);

            return app;
        }

        /// <summary>
        ///     使用健康检查端点功能
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/HealthCheck", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }

        /// <summary>
        ///     使用Swagger
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhiSwagger(this IApplicationBuilder app, DocExpansion docExpansion = DocExpansion.None)
        {
            app.UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/{Version}");
            app.UseSwaggerUI(c =>
            {
                c.EnableDeepLinking();
                c.DocExpansion(docExpansion);
                c.DisplayRequestDuration();
                c.SwaggerEndpoint($"{Assembly.GetEntryAssembly()?.GetName().Name}/v1", "V1");
            });

            return app;
        }

        /// <summary>
        ///     初始化Ehi里所有模块功能
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseEhi(this IApplicationBuilder app, Action<LibraryMiddlewareOptions> optionsAction = null)
        {
            var options = new LibraryMiddlewareOptions();
            optionsAction?.Invoke(options);

            app.UseRequestLocalization(a =>
            {
                a.SupportedCultures = options.SupportedCultures;
                a.DefaultRequestCulture = options.DefaultRequestCulture;
                a.SupportedUICultures = options.SupportedCultures;
                a.RequestCultureProviders = new List<IRequestCultureProvider> { new QueryStringRequestCultureProvider() };
            });

            //app.UseEhiApplicationInsights(options.ApplicationInsightsConfigAction);

            if (options.EnableHttpRequestLog)
            {
                app.UseEhiHttpRequestLog();
            }

            if (options.EnableCompression)
                app.UseResponseCompression();

            app.UseFeI(options.FeIApplicationBuilderAction);

            if (options.EnableSwaggerAuth)
            {
                app.UseEhiSwaggerAuth();
            }

            app.UseEhiSwagger(options.SwaggerDocExpansion);

            if (StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Admin))
            {
                app.UseAuthentication();
            }

            app.UseEhiMvc(options.RouteBuilderAction);

            app.UseEhiHealthChecks();

            if (StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Api))
            {
                app.UseEhiExceptionHandler();
            }

            return app;
        }
    }
}
