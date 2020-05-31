using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Metrics
{
    public interface IMeasureMetrics
    {
        /// <summary>计数器，会定时清空(服务端配置)</summary>
        /// <param name="statName">指标</param>
        /// <param name="value">变化值</param>
        /// <param name="sampleRate">采样率：(0,1]</param>
        /// <param name="tags">标签</param>
        void Counter([NotNull] string statName, int value = 1, double sampleRate = 1.0, IReadOnlyCollection<KeyValuePair<string, string>> tags = null);

        /// <summary>计量器，不清空</summary>
        /// <param name="statName">指标</param>
        /// <param name="value">绝对值，不能小于0</param>
        /// <param name="tags">标签</param>
        void Gauge([NotNull] string statName, double value, IReadOnlyCollection<KeyValuePair<string, string>> tags = null);
    }

    public static class MeasureMetricsExtensions
    {
        /// <summary>计数器，会定时清空(服务端配置)</summary>
        /// <param name="metrics"></param>
        /// <param name="statName">指标</param>
        /// <param name="tagKey"></param>
        /// <param name="tagValue"></param>
        /// <param name="value">变化值</param>
        /// <param name="sampleRate">采样率：(0,1]</param>
        public static void Counter([CanBeNull] this IMeasureMetrics metrics, [NotNull] string statName, string tagKey, string tagValue, int value = 1, double sampleRate = 1.0)
        {
            if (string.IsNullOrEmpty(tagKey) || string.IsNullOrEmpty(tagValue))
                metrics?.Counter(statName, value, sampleRate);
            else
                metrics?.Counter(statName, value, sampleRate, new Dictionary<string, string>
                    {
                        {tagKey, tagValue}
                    });
        }

        /// <summary>计数器，会定时清空(服务端配置)</summary>
        /// <param name="metrics"></param>
        /// <param name="statName">指标</param>
        /// <param name="value">绝对值，不能小于0</param>
        /// <param name="tagKey"></param>
        /// <param name="tagValue"></param>
        public static void Gauge([CanBeNull] this IMeasureMetrics metrics, [NotNull] string statName, double value, string tagKey, string tagValue)
        {
            if (string.IsNullOrEmpty(tagKey) || string.IsNullOrEmpty(tagValue))
                metrics?.Gauge(statName, value);
            else
                metrics?.Gauge(statName, value, new Dictionary<string, string>
                {
                    {tagKey, tagValue}
                });
        }
    }
}
