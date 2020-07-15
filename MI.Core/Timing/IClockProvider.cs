using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Timing
{
    /// <summary>
    ///     提供公用的时间操作的接口定义
    /// </summary>
    public interface IClockProvider
    {
        /// <summary>
        ///     获取当前时间
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        ///     返回当前时间类型
        /// </summary>
        DateTimeKind Kind { get; }

        /// <summary>
        ///     格式化给定的 <see cref="DateTime" />
        /// </summary>
        /// <param name="dateTime">
        ///     将要被格式化的时间
        /// </param>
        /// <returns>
        ///     已被格式化的时间
        /// </returns>
        DateTime Normalize(DateTime dateTime);
    }
}
