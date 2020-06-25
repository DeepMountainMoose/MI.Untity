using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime
{
    /// <summary>
    ///     环境范围提供程序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAmbientScopeProvider<T>
    {
        T GetValue(string contextKey);

        IDisposable BeginScope(string contextKey, T value);
    }
}
