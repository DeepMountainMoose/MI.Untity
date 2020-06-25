using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core
{
    /// <summary>
    ///     此类提供当Dispose方法调用的时候即调用action委托
    /// </summary>
    public class DisposeAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        ///     创建一个新的 <see cref="DisposeAction" /> 对象.
        /// </summary>
        /// <param name="action">
        ///     将要在disposed的时候调用的委托
        /// </param>
        public DisposeAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            _action = action;
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            _action();
        }
    }
}
