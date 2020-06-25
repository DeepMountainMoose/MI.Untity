using MI.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MI.Core.Modules
{
    internal class ModuleCollection : List<ModuleInfo>
    {
        /// <summary>
        ///     启动模块
        /// </summary>
        public Type StartupModuleType { get; }

        public ModuleCollection(Type startupModuleType)
        {
            StartupModuleType = startupModuleType;
        }

        /// <summary>
        ///     返回一个模块的引用实例.
        /// </summary>
        /// <typeparam name="TModule">模块类型</typeparam>
        /// <returns>模块的引用实例</returns>
        public TModule GetModule<TModule>() where TModule : Module
        {
            var module = this.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new ModuleException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule)module.Instance;
        }

        /// <summary>
        ///     根据模块间的依赖关系排序.
        /// </summary>
        /// <returns>排序后的模块集合</returns>
        public List<ModuleInfo> GetSortedModuleListByDependency()
        {
            var sortedModules = this.SortByDependencies(x => x.Dependencies);
            EnsureKernelModuleToBeFirst(sortedModules);
            EnsureStartupModuleToBeLast(sortedModules, StartupModuleType);
            return sortedModules;
        }

        private static void EnsureKernelModuleToBeFirst(List<ModuleInfo> modules)
        {
            var kernelModuleIndex = modules.FindIndex(m => m.Type == typeof(KernelModule));
            if (kernelModuleIndex > 0)
            {
                var kernelModule = modules[kernelModuleIndex];
                modules.RemoveAt(kernelModuleIndex);
                modules.Insert(0, kernelModule);
            }
        }

        private static void EnsureStartupModuleToBeLast(List<ModuleInfo> modules, Type startupModuleType)
        {
            var startupModuleIndex = modules.FindIndex(m => m.Type == startupModuleType);
            if (startupModuleIndex >= modules.Count - 1)
            {
                //It's already the last!
                return;
            }

            var startupModule = modules[startupModuleIndex];
            modules.RemoveAt(startupModuleIndex);
            modules.Add(startupModule);
        }

        public void EnsureKernelModuleToBeFirst()
        {
            EnsureKernelModuleToBeFirst(this);
        }

        public void EnsureStartupModuleToBeLast()
        {
            EnsureStartupModuleToBeLast(this, StartupModuleType);
        }
    }
}
