using FeI.Dependency;
using MI.Core.Configuration.Startup;
using MI.Core.Dependency;
using MI.Core.Reflection;
using MI.Core.Runtime;
using MI.Core.Runtime.Remoting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     Kernel (core) module
    ///     <para>核心模块</para>
    /// </summary>
    public sealed class KernelModule : Module
    {
        /// <summary>
        ///     PreInitialize is first run method
        ///     <para>预初始化,此方法是第一个执行的初始化方法</para>
        /// </summary>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            IocManager.RegisterType<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.RegisterType(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            //Configuration.UnitOfWork.RegisterFilter(DataFilters.SoftDelete, true);

        }

        /// <summary>
        ///     Initialize is second run method
        ///     <para>初始化,此方法是第二个执行的初始化方法</para>
        /// </summary>
        public override void Initialize()
        {
            foreach (var replaceAction in ((StartupConfiguration)Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }

            //IocManager.GetContainer().Install(new EventBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(typeof(KernelModule).GetAssembly(), new ConventionalRegistrationConfig
            {
                InstallInstallers = false
            });
        }

        ///// <summary>
        /////     PostInitialize is last run method
        /////     <para>自检,此方法是最后执行的初始化方法</para>
        ///// </summary>
        //public override void PostInitialize()
        //{
        //    RegisterMissingComponents();

        //    if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
        //    {
        //        var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
        //        workerManager.Start();
        //        workerManager.Add(IocManager.Resolve<IBackgroundJobManager>());
        //    }
        //}

        ///// <summary>
        /////     如果外部没有注册某些必须Component, 则本方法将注入默认Component的默认实现
        ///// </summary>
        //private void RegisterMissingComponents()
        //{
        //    if (!IocManager.IsRegistered<IGuidGenerator>())
        //    {
        //        IocManager.GetContainer().Register(
        //            Component
        //                .For<IGuidGenerator, SequentialGuidGenerator>()
        //                .Instance(SequentialGuidGenerator.Instance)
        //        );
        //    }

        //    IocManager.RegisterTypeIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
        //    IocManager.RegisterTypeIfNot<ICacheManager, MemoryCacheManager>();
        //    IocManager.RegisterTypeIfNot<IAuditInfoProvider, NullAuditInfoProvider>();
        //    IocManager.RegisterTypeIfNot<IBackgroundJobManager, BackgroundJobManager>();
        //    IocManager.RegisterTypeIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>();
        //    IocManager.RegisterTypeIfNot<IClockProvider, LocalClockProvider>();
        //    IocManager.RegisterTypeIfNot<IClientInfoProvider, NullClientInfoProvider>();
        //    IocManager.RegisterTypeIfNot<IPermissionChecker, NullPermissionChecker>();
        //    IocManager.RegisterTypeIfNot<IBackgroundJobStore, NullBackgroundJobStore>();
        //}

        ///// <summary>
        /////     关闭模块
        ///// </summary>
        //public override void Shutdown()
        //{
        //    if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
        //    {
        //        IocManager.Resolve<IBackgroundWorkerManager>().StopAndWaitToStop();
        //    }
        //}
    }
}
