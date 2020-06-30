using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///    将对象用<see cref="IDisposable" />进行包装
    /// </summary>
    public interface IDisposableDependencyObjectWrapper : IDisposableDependencyObjectWrapper<object>
    {
    }

    /// <summary>
    ///     将对象用<see cref="IDisposable" />进行包装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDisposableDependencyObjectWrapper<out T> : IDisposable
    {
        /// <summary>
        ///     解析返回的对象
        /// </summary>
        T Object { get; }
    }
}
