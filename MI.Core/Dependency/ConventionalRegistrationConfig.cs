using MI.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     此类用于Conventional的方式注册类的同时传递配置/选项.
    /// </summary>
    public class ConventionalRegistrationConfig : DictionayBasedConfig
    {
        /// <summary>
        ///     创建 <see cref="ConventionalRegistrationConfig" /> 对象.
        /// </summary>
        public ConventionalRegistrationConfig()
        {
            InstallInstallers = true;
        }

        /// <summary>
        ///     是否自动安装所有实现.
        ///     默认值: true.
        /// </summary>
        public bool InstallInstallers { get; set; }
    }
}
