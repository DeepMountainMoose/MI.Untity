using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     用于执行依赖注入任务的接口
    /// </summary>
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <param name="type">
        ///     需要检查是否已经注册的类型
        /// </param>
        new bool IsRegistered(Type type);

        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <typeparam name="T">
        ///     需要检查是否已经注册的类型
        /// </typeparam>
        new bool IsRegistered<T>();
    }
}
