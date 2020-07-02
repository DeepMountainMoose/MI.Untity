using MI.Core.Dependency;
using MI.Core.Runtime.Caching.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Configuration.Startup
{
    public class StartupConfiguration : DictionayBasedConfig, IStartupConfiguration
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        public StartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        public IIocManager IocManager { get; }

        ///// <summary>
        /////     身份验证设置.
        ///// </summary>
        //public IAuthorizationConfiguration Authorization { get; private set; }


        ///// <summary>
        /////     设置默认连接字符串
        ///// </summary>
        //public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        ///     缓存配置
        /// </summary>
        public ICachingConfiguration Caching { get; private set; }

        ///// <summary>
        /////     事件总线配置
        ///// </summary>
        //public IEventBusConfiguration EventBus { get; private set; }

        ///// <summary>
        /////     审查行为的配置
        ///// </summary>
        //public IAuditingConfiguration Auditing { get; private set; }

        ///// <summary>
        /////     默认工作单元配置
        ///// </summary>
        //public IUnitOfWorkDefaultOptions UnitOfWork { get; private set; }

        /// <summary>
        ///     模块配置
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        ///// <summary>
        /////     用于配置后台任务模块
        ///// </summary>
        //public IBackgroundJobConfiguration BackgroundJobs { get; private set; }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        public void ReplaceService(Type type, Action replaceAction)
        {
            ServiceReplaceActions[type] = replaceAction;
        }

        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, () => IocManager.Resolve<T>());
        }

        public void Initialize()
        {
            //UnitOfWork = IocManager.Resolve<IUnitOfWorkDefaultOptions>();
            Caching = IocManager.Resolve<ICachingConfiguration>();
            //EventBus = IocManager.Resolve<IEventBusConfiguration>();
            Modules = IocManager.Resolve<IModuleConfigurations>();
            //Auditing = IocManager.Resolve<IAuditingConfiguration>();
            //BackgroundJobs = IocManager.Resolve<IBackgroundJobConfiguration>();
            //Authorization = IocManager.Resolve<IAuthorizationConfiguration>();

            ServiceReplaceActions = new Dictionary<Type, Action>();
        }
    }
}
