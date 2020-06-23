using MI.Core.Configuration.Startup;
using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     模块的基础定义,所有模块类必须继承实现该类
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        ///     获取启动配置
        /// </summary>
        protected internal IStartupConfiguration Configuration { get; internal set; }

        /// <summary>
        ///     获取当前Ioc容器
        /// </summary>
        protected internal IIocManager IocManager { get; internal set; }

        /// <summary>
        ///     在程序启动的时候会首先执行该方法,可以在此处运行一些预初始化的代码
        /// </summary>
        public virtual void PreInitialize()
        {
        }

        /// <summary>
        ///     模块初始化
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        ///     在模块初始化之后执行的方法
        /// </summary>
        public virtual void PostInitialize()
        {
        }

        /// <summary>
        ///     当程序结束的时候执行的关闭方法
        /// </summary>
        public virtual void Shutdown()
        {
        }

        /// <summary>
        ///     检查给定的类型是否为模块类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsModule(Type type)
        {
            return type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract &&
                   typeof(Module).GetTypeInfo().IsAssignableFrom(type);
        }

        /// <summary>
        ///     搜索模块依赖
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsModule(moduleType))
                throw new ModuleException("This type is not a module: " + moduleType.AssemblyQualifiedName);

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependsOnAttribute), true))
            {
                var dependsOnAttributes =
                    moduleType.GetTypeInfo()
                        .GetCustomAttributes(typeof(DependsOnAttribute), true)
                        .Cast<DependsOnAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                    list.AddRange(dependsOnAttribute.DependedModuleTypes);
            }
            return list;
        }

        public static List<Type> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var list = new List<Type>();
            AddModuleAndDependenciesResursively(list, moduleType);
            return list;
        }

        private static void AddModuleAndDependenciesResursively(List<Type> modules, Type module)
        {
            if (!IsModule(module))
                throw new CoreException("This type is not an module: " + module.AssemblyQualifiedName);

            if (modules.Contains(module))
                return;

            modules.Add(module);

            var dependedModules = FindDependedModuleTypes(module);
            foreach (var dependedModule in dependedModules)
                AddModuleAndDependenciesResursively(modules, dependedModule);
        }
    }
}
