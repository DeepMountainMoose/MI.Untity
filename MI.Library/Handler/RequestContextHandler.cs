using MI.Common;
using MI.Core.Extensions;
using MI.Library.Enum;
using MI.Library.Interface.Common;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MI.Library.Handler
{
    public class RequestContextHandler : DelegatingHandler
    {
        public PlatformPriority PlatformPriority { get; }

        public bool IsAttachData { get; }

        private readonly ProductInfoHeaderValue _ua;

        public RequestContextHandler(PlatformPriority platformPriority, bool isAttachData)
        {
            PlatformPriority = platformPriority;
            IsAttachData = isAttachData;
            var assemblyName = typeof(ServiceClient.ServiceClient).Assembly.GetName();
            _ua = new ProductInfoHeaderValue(assemblyName.Name, assemblyName.Version.ToString());
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestContext = RequestContext.GetData();
            if (requestContext != null)
            {
                //平台和操作人是必须的
                var contextString = requestContext.OperatorId.IsNullOrEmpty() ?
                    $"Platform={GetPlatform(requestContext)}" :
                    $"Platform={GetPlatform(requestContext)}&OperatorId={requestContext.OperatorId ?? string.Empty}";

                if (!string.IsNullOrEmpty(requestContext.ChannelId))
                    contextString = string.Concat(contextString, $"&ChannelId={requestContext.ChannelId}");
                if (!string.IsNullOrEmpty(requestContext.EnterpriseId))
                    contextString = string.Concat(contextString, $"&EnterpriseId={requestContext.EnterpriseId}");
                if (!string.IsNullOrEmpty(requestContext.Remark))
                    contextString = string.Concat(contextString, $"&Remark={requestContext.Remark}");
                if (requestContext.Command != CommandType.None)
                    contextString = string.Concat(contextString, $"&Command={requestContext.CommandStr}");
                if (RequestContext.GetIsTrack)
                    contextString = string.Concat(contextString, "&IsTrack=true");

                if (string.IsNullOrEmpty(request.RequestUri.Query))
                    request.RequestUri = new Uri($"{request.RequestUri.OriginalString}?{contextString}");
                else
                    request.RequestUri = new Uri($"{request.RequestUri.OriginalString}&{contextString}");
            }

            request.Headers.UserAgent.Add(_ua);


            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        ///     获取平台,因为当前有<see cref="StartupConfig.CurrentPlatform"/>和<see cref="RequestContext"/>两种赋值方法,存在使用优先级问题
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        private string GetPlatform(RequestContext requestContext)
        {
            switch (PlatformPriority)
            {
                case PlatformPriority.CurrentPlatform:
                    return (StartupConfig.CurrentPlatform != Platform.None
                        ? (int)StartupConfig.CurrentPlatform
                        : (int)requestContext.Platform).ToString();
                case PlatformPriority.SetDataPlatform:
                    return (requestContext.Platform != Platform.None
                        ? (int)requestContext.Platform
                        : (int)StartupConfig.CurrentPlatform).ToString();
            }
            return ((int)Platform.None).ToString();
        }
    }
}
