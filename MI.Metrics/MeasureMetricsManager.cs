using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MI.Metrics
{
    public static class MeasureMetricsManager
    {
        private static IMeasureMetricsManager _metricsManager;

        public static IMeasureMetricsManager MetricsManager => _metricsManager;

        /// <summary>获取指定namespace的Metrics</summary>
        /// <param name="namespace">有限制的，不懂就选择不传参</param>
        [CanBeNull]
        public static IMeasureMetrics GetMetrics(string @namespace = null) => _metricsManager?.GetMetrics(@namespace);

        public static void SetMetricsManager([NotNull] IMeasureMetricsManager metricsManager)
        {
            if (metricsManager == null) throw new ArgumentNullException(nameof(metricsManager));

            if (Interlocked.CompareExchange(ref _metricsManager, metricsManager, null) != null)
                throw new Exception("SetMetricsManager只允许调用一次");
        }
    }
}
