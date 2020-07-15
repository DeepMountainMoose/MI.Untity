using System;

namespace MI.Core.Domain.Uow
{
    /// <summary>
    ///     用于作为<see cref="IActiveUnitOfWork.Failed" />的事件参数
    /// </summary>
    public class UnitOfWorkFailedEventArgs : EventArgs
    {
        /// <summary>
        ///     创建一个新的<see cref="UnitOfWorkFailedEventArgs" />对象.
        /// </summary>
        /// <param name="exception">导致失败的异常信息</param>
        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     导致工作单元失败的异常.
        /// </summary>
        public Exception Exception { get; }
    }
}
