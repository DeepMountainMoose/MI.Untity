using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     用于定义模块间的依赖关系.添加该attribute的类应该继承自 <see cref="Module" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DependsOnAttribute : Attribute
    {
        /// <summary>
        ///     用于定义模块间的依赖关系.
        /// </summary>
        /// <param name="dependedModuleTypes">Types of depended modules</param>
        public DependsOnAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }

        /// <summary>
        ///     依赖的模块.
        /// </summary>
        public Type[] DependedModuleTypes { get; private set; }
    }
}
