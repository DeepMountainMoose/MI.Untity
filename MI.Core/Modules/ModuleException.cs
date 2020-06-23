using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Modules
{
    /// <summary>
    ///     模块(Module)相关异常
    /// </summary>
    [Serializable]
    public class ModuleException : CoreException
    {
        /// <summary>
        ///     创建一个新的 <see cref="ModuleException" /> 对象.
        /// </summary>
        public ModuleException()
        {
        }

        /// <summary>
        ///     创建一个新的 <see cref="ModuleException" /> 对象.
        /// </summary>
        /// <param name="message">
        ///     异常消息
        /// </param>
        public ModuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     创建一个新的 <see cref="ModuleException" /> 对象.
        /// </summary>
        /// <param name="message">
        ///     异常消息
        /// </param>
        /// <param name="innerException">
        ///     内联异常
        /// </param>
        public ModuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
