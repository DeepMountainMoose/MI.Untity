using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     模块管理器
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        ///     启动模块
        /// </summary>
        /// <value></value>
        ModuleInfo StartupModule { get; }

        /// <summary>
        ///     获取当前所有模块
        /// </summary>
        IReadOnlyList<ModuleInfo> Modules { get; }

        /// <summary>
        ///     初始化所有模块
        /// </summary>
        void Initialize(Type startupModule);

        /// <summary>
        ///     启动模块
        /// </summary>
        void StartModules();

        /// <summary>
        ///     关闭所有模块
        /// </summary>
        void ShutdownModules();
    }
}
