using MI.Core.Modules;
using MI.Core.Reflection;
using MI.Web.Api.Configuration;
using MI.Web.Api.Controller;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MI.Web.Api
{
    /// <summary>
    ///     WebApi模块.
    /// </summary>
    [DependsOn(typeof(KernelModule))]
    public class WebApiModule : Module
    {
        /// <summary>
        ///     This is the first event called on application startup.
        ///     Codes can be placed here to run before dependency injection registrations.
        ///     <para>在程序启动的时候会首先执行该方法,可以在此处运行一些预初始化的代码</para>
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());
            IocManager.RegisterType<IWebApiModuleConfiguration, WebApiModuleConfiguration>();
        }

        /// <summary>
        ///     This method is used to register dependencies for this module.
        ///     <para>模块初始化</para>
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WebApiModule).GetAssembly());

        }

        /// <summary>
        ///     This method is called lastly on application startup.
        ///     <para>在模块初始化之后执行的方法</para>
        /// </summary>
        public override void PostInitialize()
        {
            var httpConfiguration = IocManager.Resolve<IWebApiModuleConfiguration>().HttpConfiguration;
            InitializeServices(httpConfiguration);
        }

        /// <summary>
        ///     初始化Web Api的Controller生成配置.
        /// </summary>
        private void InitializeServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new ApiControllerActionSelector());
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), new ApiControllerActivator(IocManager));
        }
    }
}
