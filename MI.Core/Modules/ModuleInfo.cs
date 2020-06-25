using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     模块信息
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        ///     创建一个新的<see cref="ModuleInfo" />.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public ModuleInfo(Type type, Module instance)
        {
            Type = type;
            Instance = instance;
            Assembly = Type.Assembly;

            Dependencies = new List<ModuleInfo>();
        }

        /// <summary>
        ///     包含模块定义的程序集.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        ///     模块类型.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     模块实例.
        /// </summary>
        public Module Instance { get; private set; }

        /// <summary>
        ///     本模块所依赖的其他模块.
        /// </summary>
        public List<ModuleInfo> Dependencies { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", Type.AssemblyQualifiedName);
        }
    }
}
