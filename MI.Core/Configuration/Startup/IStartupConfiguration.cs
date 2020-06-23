using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Configuration.Startup
{
    /// <summary>
    ///     用于初始化配置模块
    /// </summary>
    public interface IStartupConfiguration : IDictionaryBasedConfig
    {
        /// <summary>
        ///     获取该配置向关联的Ioc容器.
        /// </summary>
        IIocManager IocManager { get; }

        ///// <summary>
        /////     获取或者设置ORM模块的默认连接字符串.
        ///// </summary>
        //string DefaultNameOrConnectionString { get; set; }

        ///// <summary>
        /////     缓存的配置.
        ///// </summary>
        //ICachingConfiguration Caching { get; }

        ///// <summary>
        /////     用于配置<see cref="IEventBus" />.
        ///// </summary>
        //IEventBusConfiguration EventBus { get; }

        ///// <summary>
        /////     审查行为的配置.
        ///// </summary>
        //IAuditingConfiguration Auditing { get; }

        ///// <summary>
        /////     默认工作单元的配置.
        ///// </summary>
        //IUnitOfWorkDefaultOptions UnitOfWork { get; }

        ///// <summary>
        /////     模块的配置.
        ///// </summary>
        //IModuleConfigurations Modules { get; }

        ///// <summary>
        /////     用于配置后台任务模块.
        ///// </summary>
        //IBackgroundJobConfiguration BackgroundJobs { get; }

        ///// <summary>
        /////     获取对应的配置对象.
        ///// </summary>
        //T Get<T>();

        ///// <summary>
        /////     用于替换一个类的实现
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="replaceAction"></param>
        //void ReplaceService(Type type, Action replaceAction);
    }
}
