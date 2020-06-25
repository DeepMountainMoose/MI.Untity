using Castle.Windsor.MsDependencyInjection;
using MI.Core;
using MI.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using MI.Core.Dependency;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MI.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 集成FeI到AspNet Core.
        /// </summary>
        /// <remarks>此操作必须要在ConfigureServices里的最后一个执行,并且return其返回值</remarks>
        /// <code>
        /// public IServiceProvider ConfigureServices(IServiceCollection services)
        /// {
        /// //do something
        /// return services.AddFeI&lt;XXXModule&gt;();
        /// }
        /// </code>
        /// <typeparam name="TStartupModule">设置的启动模块. 模块应该是继承自<see cref="Module"/>的类.</typeparam>i
        /// <param name="services">Services.</param>
        /// <param name="optionsAction"></param>
        public static IServiceProvider AddFeI<TStartupModule>(this IServiceCollection services, Action<BootstrapperOptions> optionsAction = null)
            where TStartupModule : Module
        {
            var bootstrapper = AddBootstrapper<TStartupModule>(services, optionsAction);

            ConfigureAspNetCore(services);

            return WindsorRegistrationHelper.CreateServiceProvider(bootstrapper.IocManager.GetContainer(), services);
        }

        private static void ConfigureAspNetCore(IServiceCollection services)
        {
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            ////Use DI to create controllers
            //services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            ////Use DI to create view components
            ////services.Replace(ServiceDescriptor.Singleton<IViewComponentActivator, ServiceBasedViewComponentActivator>());

            ////Configure JSON serializer
            //services.Configure<MvcJsonOptions>(jsonOptions =>
            //{
            //    jsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //    jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});

            //Configure MVC
            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddFeI(services);
            });
        }

        private static Bootstrapper AddBootstrapper<TStartupModule>(IServiceCollection services, Action<BootstrapperOptions> optionsAction)
            where TStartupModule : Module
        {
            var bootstrapper = Bootstrapper.Create<TStartupModule>(optionsAction);
            services.AddSingleton(bootstrapper);
            return bootstrapper;
        }
    }
}
