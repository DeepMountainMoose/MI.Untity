using MI.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Entity
{
#if NETSTANDARD2
    public class AuditLog : FeI.Domain.Entities.Entity
#else 
    public class AuditLog
#endif
    {
        /// <summary>
        ///     用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     服务名称（类名或者接口名）
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        ///     Executed method name.
        ///     <para>执行的方法名</para>
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        ///     调用参数
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        ///     调用执行开始时间
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        ///     执行持续时间(毫秒)
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        ///     调用客户端的Ip地址
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        ///     客户端浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        ///     自定义信息
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        ///     异常信息对象, 如果方法执行的时候抛出异常的话则异常信息将存于此处
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        ///     操作Id
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        ///     返回值
        /// </summary>
        public string ReturnValue { get; set; }

        /// <summary>
        ///     当前平台
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        ///     当前机器名
        /// </summary>
        public string MachineName { get; set; }

#if NETSTANDARD2_1
        public string ErrorMessage { get; set; }

        public string ErrorStackTrace { get; set; }

        public string EnterpriseId { get; set; }

        public string ChannelId { get; set; }

        public string OperatorId { get; set; }
#endif

    }
}
