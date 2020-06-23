using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     Ioc的惯例注册上下文
    /// </summary>
    internal class ConventionalRegistrationContext : IConventionalRegistrationContext
    {
        internal ConventionalRegistrationContext(Assembly assembly, IIocManager iocManager,
            ConventionalRegistrationConfig config)
        {
            Assembly = assembly;
            IocManager = iocManager;
            Config = config;
        }

        /// <summary>
        ///     获取将要被注册的程序集
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        ///     获取将要注册类型的Ioc容器的引用
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     注册配置
        /// </summary>
        public ConventionalRegistrationConfig Config { get; }
    }
}
