using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     工作单元配置选项
    /// </summary>
    public class UnitOfWorkOptions : ICloneable
    {
        /// <summary>
        ///     创建一个新的<see cref="UnitOfWorkOptions" />对象
        /// </summary>
        public UnitOfWorkOptions()
        {
            FilterOverrides = new List<DataFilterConfiguration>();
        }

        /// <summary>
        ///     事务范围
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        ///     是否是事务性
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        ///     工作单元的超时时间
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        ///     如果当前工作单元是事务性的, 此属性表示事务的隔离级别.
        ///         非事务性的则此配置无效.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        ///     当前工作单元如果是用于async方法的话将会被设置为<see cref="TransactionScopeAsyncFlowOption.Enabled" />
        /// </summary>
        public TransactionScopeAsyncFlowOption? AsyncFlowOption { get; set; }

        /// <summary>
        ///     用于启用或者禁用过滤器
        /// </summary>
        public List<DataFilterConfiguration> FilterOverrides { get; set; }

        /// <summary>
        ///     返回工作单元配置的浅克隆副本
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        internal void FillDefaultsForNonProvidedOptions(IUnitOfWorkDefaultOptions defaultOptions)
        {
            if (!IsTransactional.HasValue)
            {
                IsTransactional = defaultOptions.IsTransactional;
            }

            if (!Scope.HasValue)
            {
                Scope = defaultOptions.Scope;
            }

            if (!Timeout.HasValue && defaultOptions.Timeout.HasValue)
            {
                Timeout = defaultOptions.Timeout.Value;
            }

            if (!IsolationLevel.HasValue && defaultOptions.IsolationLevel.HasValue)
            {
                IsolationLevel = defaultOptions.IsolationLevel.Value;
            }
        }
    }
}
