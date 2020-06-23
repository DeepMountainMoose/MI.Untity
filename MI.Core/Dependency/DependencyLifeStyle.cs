using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     依赖注入的生命周期.
    /// </summary>
    public enum DependencyLifeStyle
    {
        /// <summary>
        ///     单例对象.在第一次获取对象时创建对象之后获取的都是同一个实例.
        /// </summary>
        Singleton,

        /// <summary>
        ///     瞬时对象.每次获取都创建一个新的对象.
        /// </summary>
        Transient
    }
}
