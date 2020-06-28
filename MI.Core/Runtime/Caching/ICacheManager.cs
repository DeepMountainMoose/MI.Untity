using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     一个封装 <see cref="ICache" /> 的上层容器.
    ///     缓存管理器应该是单例运行,用于管理和跟踪 <see cref="ICache" />
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        ///     返回当前所有缓存实例.
        /// </summary>
        /// <returns>缓存的只读列表</returns>
        IReadOnlyList<ICache> GetAllCaches();

        /// <summary>
        ///     获取缓存.如果该缓存不存在则创建新缓存实例
        /// </summary>
        /// <param name="name">
        ///     缓存的唯一标识名字
        /// </param>
        /// <returns>
        ///     缓存的引用
        /// </returns>
        ICache GetCache(string name);
    }
}
