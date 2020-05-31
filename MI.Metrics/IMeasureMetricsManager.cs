using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Metrics
{
    public interface IMeasureMetricsManager
    {
        /// <summary>获取指定namespace的Metrics</summary>
        /// <param name="namespace">有限制的，不懂就选择不传参</param>
        IMeasureMetrics GetMetrics(string @namespace = null);
    }
}
