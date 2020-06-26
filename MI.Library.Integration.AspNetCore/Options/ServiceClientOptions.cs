using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MI.Library.Integration.AspNetCore.Options
{
    public class ServiceClientOptions
    {
        /// <summary>
        ///     弹性Http请求超时时间
        ///     <remarks>
        ///         <see cref="Interface.IResilientServiceClient" /> 将在此指定的时间超时然后触发重试,由于
        ///         <see cref="Interface.IResilientServiceClient" /> 同时会受到 <see cref="ServiceClients.ServiceClient" />
        ///         的超时时间的约束,所以超过<see cref="ServiceClients.ServiceClient.Timeout" />的设置将毫无意义
        ///     </remarks>
        /// </summary>
        public int ResilientHttpTimeout { get; set; } = 8;

        /// <summary>
        ///     <see cref="Interface.IResilientServiceClient"/> 的重试次数
        /// </summary>
        public int ResilientHttpRetryCount { get; set; } = 2;

        /// <summary>
        ///     是否启用对Http请求的跟踪
        ///     <remarks>
        ///         仅对由依赖注入获取到的所有基于<see cref="ServiceClients.IServiceClient" />有效
        ///     </remarks>
        /// </summary>
        public bool EnableHttpTrackRequest { get; set; } = true;

        /// <summary>
        ///     Http是否支持使用代理
        /// <remarks>
        ///     默认值根据是否附加调试器决定，调试的时候使用Proxy，否则不使用
        /// </remarks>
        /// </summary>
        public Func<bool> IsUseProxy { get; set; } = () => Debugger.IsAttached;
    }
}
