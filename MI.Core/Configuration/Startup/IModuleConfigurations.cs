using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Configuration.Startup
{
    /// <summary>
    ///     用于配置模块.
    ///     可创建对应于此类的扩展方法的形式代替重写<see cref="IStartupConfiguration.Modules" />
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        ///     获取当前配置对象
        /// </summary>
        IStartupConfiguration Configuration { get; }
    }
}
