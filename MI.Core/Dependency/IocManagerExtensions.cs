using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     Ioc容器扩展方法
    /// </summary>
    public static class IocManagerExtensions
    {
        /// <summary>
        ///     返回 <see cref="IIocManager" /> 的依赖注入容器, 要求 <see cref="IIocManager" /> 必须是实现自 <see cref="IocManager" />
        /// </summary>
        /// <param name="iocManager"></param>
        /// <returns></returns>
        public static IWindsorContainer GetContainer(this IIocManager iocManager)
        {
            var manager = iocManager as IocManager;
            if (manager != null)
                return manager.IocContainer;
            return null;
        }
    }
}
