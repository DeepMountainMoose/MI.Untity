using EnumsNET;
using MI.Library.Interface;
using MI.Library.Interface.Common;
using ServiceClient;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MI.Library.Integration.AspNetCore.Service
{
    public class ResilientServiceClient : ServiceClientBase, IResilientServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResilientServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            IsThrow = StartupConfig.StartupMode.HasAnyFlags(StartupModeType.Ui) ? false : true;
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _httpClientFactory.CreateClient(nameof(IResilientServiceClient)).SendAsync(request);
        }

        /// <summary>
        ///     获取Http响应
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpVerb"></param>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> GetHttpResponseMessage(string url, HttpVerb httpVerb,
            HttpContent body, CancellationToken cancellationToken)
        {
            HttpResponseMessage result;
            var innerHttpClient = _httpClientFactory.CreateClient(nameof(IResilientServiceClient));
            switch (httpVerb)
            {
                case HttpVerb.Get:
                    result =
                        await
                            innerHttpClient.GetAsync(url, cancellationToken);
                    break;
                case HttpVerb.Post:
                    result =
                        await
                            innerHttpClient.PostAsync(url, body, cancellationToken);
                    break;
                case HttpVerb.Put:
                    result =
                        await innerHttpClient.PutAsync(url, body, cancellationToken);
                    break;
                case HttpVerb.Patch:
                    result = await PatchAsync(innerHttpClient, url, body, cancellationToken);
                    break;
                case HttpVerb.Delete:
                    result =
                        await
                            innerHttpClient.DeleteAsync(url, cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(httpVerb.ToString(), httpVerb, null);
            }

            return result;
        }

        public override void Dispose()
        {
            //nothing
        }

        public override HttpRequestHeaders DefaultRequestHeaders => throw new NotSupportedException();
        public override TimeSpan Timeout => throw new NotSupportedException();

        #region [ Private Method ]
        private Task<HttpResponseMessage> PatchAsync(HttpClient httpClient, string requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            return httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        #endregion
    }
}
