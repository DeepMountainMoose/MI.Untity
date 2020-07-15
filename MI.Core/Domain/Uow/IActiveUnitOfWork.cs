using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     此接口用于代表当前处于激活状态下的工作单元.
    ///     此接口不能被注入
    /// </summary>
    public interface IActiveUnitOfWork
    {
        /// <summary>
        ///     获取当前工作单元的配置选项.
        /// </summary>
        UnitOfWorkOptions Options { get; }

        /// <summary>
        ///     当前工作单元是否已经被释放
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        ///     Gets data filter configurations for this unit of work.
        ///     <para>
        ///         获取当前工作单元的数据过滤配置
        ///     </para>
        /// </summary>
        IReadOnlyList<DataFilterConfiguration> Filters { get; }

        /// <summary>
        ///     当工作单元提交成功完成的时候触发的事件.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        ///     当工作单元提交失败的时候触发的时间.
        /// </summary>
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <summary>
        ///     当工作单元被释放的时候触发的事件.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        ///     保存当前工作单元的所有的更改
        ///     可以在需要的时候直接调用此方法
        ///     <para>
        ///         注意: 如果当前工作单元是事务性的, 那么如果事务发生回滚则该方法也会对应回滚
        ///     </para>
        /// </summary>
        void SaveChanges();

        /// <summary>
        ///     保存当前工作单元的所有的更改
        ///     可以在需要的时候直接调用此方法
        ///     <para>
        ///         注意: 如果当前工作单元是事务性的, 那么如果事务发生回滚则该方法也会对应回滚
        ///     </para>
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        ///     检查指定过滤器是否已启用
        /// </summary>
        /// <param name="filterName">过滤器名称. 应该定义于<see cref="DataFilters"/>里.</param>
        bool IsFilterEnabled(string filterName);
    }
}
