using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Reflection
{
    /// <summary>
    ///     类型探测器
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        ///     返回符合条件的类型
        /// </summary>
        /// <param name="predicate">类型搜索条件</param>
        /// <returns>符合条件的类型数组</returns>
        Type[] Find(Func<Type, bool> predicate);

        /// <summary>
        ///     返回所有类型
        /// </summary>
        /// <returns>类型数组</returns>
        Type[] FindAll();
    }
}
