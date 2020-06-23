using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    public sealed class BasicConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <summary>
        ///     程序集注册方法
        /// </summary>
        /// <param name="context">注册的程序集上下文</param>
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            //Transient
            context.IocManager.GetContainer().Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ITransientDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleTransient()
                );

            //Singleton
            context.IocManager.GetContainer().Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ISingletonDependency>()
                    .WithService.Self()
                    .WithService.DefaultInterfaces()
                    .LifestyleSingleton()
                );

        }
    }
}
