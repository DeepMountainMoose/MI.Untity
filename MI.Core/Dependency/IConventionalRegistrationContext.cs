using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     使对象可以在注册的时候添加惯例处理
    /// </summary>
    public interface IConventionalRegistrationContext
    {
        /// <summary>
        ///     将要被注册的程序集
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        ///     将要注册类型的Ioc容器引用
        /// </summary>
        IIocManager IocManager { get; }

        /// <summary>
        ///     注册配置
        /// </summary>
        ConventionalRegistrationConfig Config { get; }
    }
}
