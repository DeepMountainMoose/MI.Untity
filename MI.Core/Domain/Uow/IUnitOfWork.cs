using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///  工作单元的定义.
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle
    {
        /// <summary>
        ///  表示工作单元的唯一Id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///  引用外的工作单元.(如果存在)
        /// </summary>
        IUnitOfWork Outer { get; set; }

        /// <summary>
        ///  通过给定的配置开始一个工作单元.
        /// </summary>
        /// <param name="options">工作单元配置</param>
        void Begin(UnitOfWorkOptions options);
    }
}
