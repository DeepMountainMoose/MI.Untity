using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Util
{
    /// <summary>
    ///     此类遵循Null Pattern模式定义一个空的实现<see cref="IDisposable" />的类代表其空模式实例
    /// </summary>
    internal sealed class NullDisposable : IDisposable
    {
        private NullDisposable()
        {
        }

        public static NullDisposable Instance { get; } = new NullDisposable();

        public void Dispose()
        {
            //Do nothing
        }
    }
}
