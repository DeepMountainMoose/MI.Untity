using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Configuration.Startup
{
    /// <summary>
    ///     <see cref="IStartupConfiguration"/>的扩展方法
    /// </summary>
    public static class StartupConfigurationExtensions
    {
        /// <summary>
        /// 用于替换一个服务类
        /// </summary>
        /// <param name="configuration">配置类</param>
        /// <param name="type">服务的类型</param>
        /// <param name="impl">服务的实现类型</param>
        /// <param name="lifeStyle">生命周期</param>
        public static void ReplaceService(this IStartupConfiguration configuration, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            configuration.ReplaceService(type, () =>
            {
                configuration.IocManager.RegisterType(type, impl, lifeStyle);
            });
        }

        /// <summary>
        /// 用于替换一个服务类
        /// </summary>
        /// <typeparam name="TType">服务的类型</typeparam>
        /// <typeparam name="TImpl">服务的实现类型</typeparam>
        /// <param name="configuration">配置类</param>
        /// <param name="lifeStyle">生命周期</param>
        public static void ReplaceService<TType, TImpl>(this IStartupConfiguration configuration, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            configuration.ReplaceService(typeof(TType), () =>
            {
                configuration.IocManager.RegisterType<TType, TImpl>(lifeStyle);
            });
        }


        /// <summary>
        /// 用于替换一个服务类
        /// </summary>
        /// <typeparam name="TType">服务的类型</typeparam>
        /// <param name="configuration">配置类</param>
        /// <param name="replaceAction">替换操作</param>
        public static void ReplaceService<TType>(this IStartupConfiguration configuration, Action replaceAction)
            where TType : class
        {
            configuration.ReplaceService(typeof(TType), replaceAction);
        }
    }
}
