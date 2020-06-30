using Castle.MicroKernel.Registration;
using MI.Core.Dependency;
using System.Web.Http;

namespace MI.Web.Api.Controller
{
    /// <summary>
    ///     Web Api的注册惯例(Conventional)
    /// </summary>
    /// <remarks>用于将<see cref="ApiController"/>都添加到依赖注入器里</remarks>
    public class ApiControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <summary>
        ///     程序集注册
        /// </summary>
        /// <param name="context"></param>
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.GetContainer().Register(
                Classes.FromAssembly(context.Assembly)
                    .BasedOn<ApiController>()
                    .LifestyleTransient()
                );
        }
    }
}
