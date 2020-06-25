using Castle.Core.Logging;
using MI.Core.Configuration.Startup;
using MI.Core.Dependency;
using MI.Core.Extensions;
using MI.Core.PlugIns;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MI.Core.Modules
{
    [DebuggerDisplay("Module Count {_modules.Count}")]
    public class ModuleManager : IModuleManager
    {
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public ModuleInfo StartupModule { get; private set; }

        /// <inheritdoc />
        public IReadOnlyList<ModuleInfo> Modules => _modules.ToImmutableList();

        private ModuleCollection _modules;

        private readonly IIocManager _iocManager;
        private readonly IPlugInManager _plugInManager;

        public ModuleManager(IIocManager iocManager, IPlugInManager plugInManager)
        {
            _iocManager = iocManager;
            _plugInManager = plugInManager;
            Logger = NullLogger.Instance;
        }

        /// <inheritdoc />
        public virtual void Initialize(Type startupModule)
        {
            _modules = new ModuleCollection(startupModule);
            LoadAllModules();
        }

        /// <inheritdoc />
        public virtual void ShutdownModules()
        {
            Logger.Debug("Shutting down has been started");
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());
            Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.Debug("Loading modules...");

            var moduleTypes = FindAllModules().Distinct().ToList();

            Logger.Debug("Found " + moduleTypes.Count + " modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes);

            _modules.EnsureKernelModuleToBeFirst();
            _modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", _modules.Count);
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                _iocManager.RegisterTypeIfNot(moduleType);
            }
        }

        private List<Type> FindAllModules()
        {
            var modules = Module.FindDependedModuleTypesRecursivelyIncludingGivenModule(_modules.StartupModuleType);
            modules.AddIfNotContains(typeof(KernelModule));

            _plugInManager
                .PlugInSources
                .GetAllModules()
                .ForEach(m => modules.AddIfNotContains(m));

            return modules;
        }

        /// <inheritdoc />
        public virtual void StartModules()
        {
            var sortedModules = _modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        private void CreateModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                if (!(_iocManager.Resolve(moduleType) is Module moduleObject))
                {
                    throw new ModuleException("This type is not an module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = _iocManager;
                moduleObject.Configuration = _iocManager.Resolve<IStartupConfiguration>();

                var moduleInfo = new ModuleInfo(moduleType, moduleObject);

                _modules.Add(moduleInfo);

                if (moduleType == _modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in _modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in Module.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = _modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new CoreException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
