using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Timing
{
    /// <summary>
    ///     基于本地时间实现的 <see cref="IClockProvider" /> 
    /// </summary>
    public class LocalClockProvider : IClockProvider
    {
        /// <summary>
        ///     获取当前时间
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        ///     返回当前时间类型
        /// </summary>
        public DateTimeKind Kind => DateTimeKind.Local;

        /// <summary>
        ///     格式化给定的 <see cref="DateTime" />
        /// </summary>
        /// <param name="dateTime">
        ///     将要被格式化的时间
        /// </param>
        /// <returns>
        ///     已被格式化的时间
        /// </returns>
        public DateTime Normalize(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }

            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            return dateTime;
        }
    }
}
