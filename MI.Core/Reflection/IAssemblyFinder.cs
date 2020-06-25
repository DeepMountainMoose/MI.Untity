using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Reflection
{
    /// <summary>
    ///     程序集探测器接口
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        ///     返回当前程序的所有程序集
        /// </summary>
        /// <returns>
        ///     当前程序下的所有程序集列表
        /// </returns>
        List<Assembly> GetAllAssemblies();
    }
}
