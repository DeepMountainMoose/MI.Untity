using System;

namespace MI.Core.Timing
{
    /// <summary>
    ///     提供执行常见的日期-时间操作
    /// </summary>
    public static class Clock
    {
        private static IClockProvider _provider;

        static Clock()
        {
            Provider = new LocalClockProvider();
        }

        /// <summary>
        ///     使用此对象来执行所有 <see cref="Clock" /> 操作.
        ///         默认值: <see cref="LocalClockProvider" />.
        /// </summary>
        public static IClockProvider Provider
        {
            get { return _provider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Can not set Clock.Provider to null!");

                _provider = value;
            }
        }

        /// <summary>
        ///     获取 <see cref="Provider" /> 的当前时间.
        /// </summary>
        public static DateTime Now => Provider.Now;

        /// <summary>
        ///     指定当前 <see cref="T:System.DateTime" />对象代表的是本地时间,世界通用时间还是其它类型的时间
        /// </summary>
        public static DateTimeKind Kind => Provider.Kind;

        /// <summary>
        ///     通过当前的 <see cref="Provider" /> 格式化指定的 <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateTime">
        ///     将要被格式化的时间
        /// </param>
        /// <returns>
        ///     已被格式化的时间
        /// </returns>
        public static DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }
    }
}
