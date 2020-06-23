using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     用于代表惯例注册的接口
    /// </summary>
    /// <remarks>
    ///     实现此接口并注册到
    ///         <see>
    ///             <cref>IIocManager.AddConventionalRegistrar</cref>
    ///         </see>
    ///         方法将能够通过自己定义的惯例修饰。
    /// </remarks>
    public interface IConventionalDependencyRegistrar
    {
        /// <summary>
        ///     通过惯例来注册程序集
        /// </summary>
        /// <param name="context">注册上下文</param>
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}
