using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Runtime.Caching
{
    /// <summary>
    ///     缓存过期类型
    /// </summary>
    public enum CacheExpireType
    {
        /// <summary>
        ///     绝对过期类型
        /// </summary>
        Absolute,

        /// <summary>
        ///     滑动过期类型
        /// </summary>
        Slide
    }
}
