using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     数据过滤配置
    /// </summary>
    public class DataFilterConfiguration
    {
        /// <summary>
        ///     过滤配置的名称
        /// </summary>
        public string FilterName { get; private set; }

        /// <summary>
        ///     当前是否启用
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        ///     过滤参数
        /// </summary>
        public IDictionary<string, object> FilterParameters { get; private set; }

        /// <summary>
        ///     创建数据过滤实例
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="isEnabled"></param>
        public DataFilterConfiguration(string filterName, bool isEnabled)
        {
            FilterName = filterName;
            IsEnabled = isEnabled;
            FilterParameters = new Dictionary<string, object>();
        }

        internal DataFilterConfiguration(DataFilterConfiguration filterToClone)
            : this(filterToClone.FilterName, filterToClone.IsEnabled)
        {
            foreach (var filterParameter in filterToClone.FilterParameters)
            {
                FilterParameters[filterParameter.Key] = filterParameter.Value;
            }
        }
    }
}
