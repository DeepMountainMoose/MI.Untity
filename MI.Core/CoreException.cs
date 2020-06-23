using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MI.Core
{
    /// <summary>
    ///     核心系统的异常基类.
    /// </summary>
    [Serializable]
    public class CoreException : Exception
    {
        /// <summary>
        ///     创建一个新的 <see cref="CoreException" /> 对象.
        /// </summary>
        public CoreException()
        {
        }

        /// <summary>
        ///     创建一个新的 <see cref="CoreException" /> 对象.
        /// </summary>
        /// <param name="message">异常消息</param>
        public CoreException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     创建一个新的 <see cref="CoreException" /> 对象.
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内联异常</param>
        public CoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        /// <summary>
        ///     创建一个新的 <see cref="CoreException" /> 对象.
        /// </summary>
        public CoreException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}
