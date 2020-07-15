using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///  用于完成一个工作单元的Handle.
    /// </summary>
    public interface IUnitOfWorkCompleteHandle : IDisposable
    {
        /// <summary>
        ///     提交并完成当前的工作单元.
        ///     此操作将保存所有的变更并提交事务(如果存在)
        /// </summary>
        void Complete();

        /// <summary>
        ///     提交并完成当前的工作单元.
        ///     此操作将保存所有的变更并提交事务(如果存在)
        /// </summary>
        Task CompleteAsync();
    }
}
