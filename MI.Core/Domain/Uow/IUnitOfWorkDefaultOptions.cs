using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     默认的工作单元配置接口.
    /// </summary>
    public interface IUnitOfWorkDefaultOptions
    {
        /// <summary>
        ///     事务范围.
        /// </summary>
        TransactionScopeOption Scope { get; set; }

        /// <summary>
        ///     工作单元应该是事务性的.
        ///     Default: true.
        /// </summary>
        bool IsTransactional { get; set; }

        /// <summary>
        ///     获取或者设置超时时间.
        /// </summary>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        ///     获取或者设置事务隔离级别.
        ///     当 <see cref="IsTransactional" /> 为true的时候才有效.
        /// </summary>
        IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        ///     Gets list of all data filter configurations.
        ///     <para>返回所有数据过滤配置</para>
        /// </summary>
        IReadOnlyList<DataFilterConfiguration> Filters { get; }

        /// <summary>
        /// 当前工作单元的选择列表
        /// </summary>
        List<Func<Type, bool>> ConventionalUowSelectors { get; }

        /// <summary>
        ///     Registers a data filter to unit of work system.
        ///     <para>注册一个数据过滤器到工作单元系统中</para>
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="isEnabledByDefault">Is filter enabled by default.</param>
        void RegisterFilter(string filterName, bool isEnabledByDefault);

        /// <summary>
        ///     Overrides a data filter definition.
        ///     <para>重写一个数据过滤器的定义</para>
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="isEnabledByDefault">Is filter enabled by default.</param>
        void OverrideFilter(string filterName, bool isEnabledByDefault);
    }
}
