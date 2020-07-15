using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     获取或者设置当前的 <see cref="IUnitOfWork" />.
    /// </summary>
    public interface ICurrentUnitOfWorkProvider
    {
        /// <summary>
        ///     获取或者设置当前的 <see cref="IUnitOfWork" />.
        /// </summary>
        IUnitOfWork Current { get; set; }
    }
}
