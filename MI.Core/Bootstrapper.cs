using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using JetBrains.Annotations;
using MI.Core.Configuration.Startup;
using MI.Core.Dependency;
using MI.Core.Modules;
using MI.Core.PlugIns;
using System;
using System.Reflection;
using Module = MI.Core.Modules.Module;

namespace MI.Core
{
    /// <summary>
    ///     此类为整个系统的启动类.
    ///     必须在应用程序中首先实例化本类并执行<see cref="Initialize" />方法.
    /// </summary>
    public class Bootstrapper : IDisposable
    {
        private IModuleManager _moduleManager;
        private ILogger _logger;

        /// <summary>
        ///     当前启动器是否已经被释放资源
        /// </summary>
        protected bool IsDisposed;

        /// <summary>
        /// 当前插件列表
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        /// <summary>
        /// 获取当前用于启动应用的模块类型
        /// </summary>
        public Type StartupModule { get; }

        /// <summary>
        /// 创建一个<see cref="Bootstrapper"/> 实例.
        /// </summary>
        /// <param name="startupModule">启动应用程序所使用的模块. 此类型应该继承自<see cref="Module"/>.</param>
        /// <param name="optionsAction">配置方法</param>
        private Bootstrapper([NotNull] Type startupModule, [CanBeNull] Action<BootstrapperOptions> optionsAction = null)
        {
            Check.NotNull(startupModule, nameof(startupModule));

            var options = new BootstrapperOptions();
            optionsAction?.Invoke(options);

            if (!typeof(Module).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(Module)}.");
            }

            StartupModule = startupModule;

            IocManager = options.IocManager;
            PlugInSources = options.PlugInSources;

            _logger = NullLogger.Instance;

            //if (!options.DisableAllInterceptors)
            //{
            //    AddInterceptorRegistrars();
            //}
        }

        /// <summary>
        /// 创建<see cref="Bootstrapper"/>实例.
        /// </summary>
        /// <typeparam name="TStartupModule">启动模块. 此类应该要继承自<see cref="Modules.Module"/>.</typeparam>
        public static Bootstrapper Create<TStartupModule>([CanBeNull] Action<BootstrapperOptions> optionsAction = null)
            where TStartupModule : Module
        {
            return new Bootstrapper(typeof(TStartupModule), optionsAction);
        }

        /// <summary>
        /// 创建<see cref="Bootstrapper"/>实例.
        /// </summary>
        /// <param name="startupModule">启动模块. 此类应该要继承自<see cref="Modules.Module"/>.</param>
        /// <param name="optionsAction">启动配置</param>
        public static Bootstrapper Create([NotNull] Type startupModule, [CanBeNull] Action<BootstrapperOptions> optionsAction = null)
        {
            return new Bootstrapper(startupModule, optionsAction);
        }

        /// <summary>
        ///     获取依赖注入管理器的引用.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     释放当前系统
        ///     <para>执行此方法后所有已加载的模块将会被关闭</para>
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            _moduleManager?.ShutdownModules();
        }


        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(Bootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<Bootstrapper>())
            {
                IocManager.GetContainer().Register(
                    Component.For<Bootstrapper>().Instance(this)
                    );
            }
        }

        private void AddInterceptorRegistrars()
        {
            //AuditingInterceptorRegistrar.Initialize(IocManager);
            //UnitOfWorkRegistrar.Initialize(IocManager);
            //AuthorizationInterceptorRegistrar.Initialize(IocManager);
        }

        /// <summary>
        ///     初始化所有系统
        /// </summary>
        /// <exception cref="ModuleException">模块初始化错误</exception>
        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();
                IocManager.GetContainer().Install(new CoreInstaller());

                IocManager.Resolve<PlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<StartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<ModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }
    }
}
