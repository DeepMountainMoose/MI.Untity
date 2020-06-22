using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceClient
{
    /// <summary>
    ///     网络请求服务客户端
    ///     <para>对<see cref="System.Net.Http.HttpClient" />的一层封装,提供各种对REST风格的API请求操作</para>
    ///     <para>该类底层请求使用了<see cref="System.Net.Http.HttpClient" />,其请求方法均为线程安全</para>
    ///     <remarks>可使用<see cref="Default" />获取默认实例</remarks>
    /// </summary>
    public class ServiceClient : ServiceClientBase
    {
        #region [ Field ]

        private static ServiceClient _defaultInstance;
        private static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 20);

        #endregion

        #region [ Property ]

        /// <summary>
        ///     默认网络请求客户端实例
        ///     <para>由于<see cref="HttpClient" />是为线程安全的类,所以本类以静态单例的模式提供服务也同样是线程安全的</para>
        /// </summary>
        public static ServiceClient Default => _defaultInstance ?? (_defaultInstance = new ServiceClient());

        /// <summary>
        ///     内部的<see cref="HttpClient" />实例
        /// </summary>
        protected HttpClient InnerHttpClient { get; }

        /// <summary>
        ///     超时时间
        /// </summary>
        public override TimeSpan Timeout
        {
            get => InnerHttpClient.Timeout;
        }

        /// <summary>
        ///     默认Http请求头
        /// </summary>
        public override HttpRequestHeaders DefaultRequestHeaders => InnerHttpClient.DefaultRequestHeaders;

        #endregion

        #region [ Ctor ]
        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        public ServiceClient() : this(null, DefaultTimeout)
        {
        }

        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        /// <param name="handler">Http消息处理程序</param>
        public ServiceClient(HttpMessageHandler handler) : this(null, DefaultTimeout, handler)
        {
        }

        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        /// <param name="baseAddress">请求地址的基地址</param>
        public ServiceClient(Uri baseAddress) : this(baseAddress, DefaultTimeout)
        {
        }

        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="timeout"></param>
        /// <param name="handler"></param>
        public ServiceClient(Uri baseAddress, TimeSpan timeout, HttpMessageHandler handler = null)
        {
            handler = handler ?? new HttpClientHandler();
            IsThrow = true;

            InnerHttpClient = new HttpClient(handler)
            {
                BaseAddress = baseAddress,
                Timeout = timeout
            };
        }

        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="handler"></param>
        public ServiceClient(TimeSpan timeout, HttpMessageHandler handler = null)
        {
            handler = handler ?? new HttpClientHandler();
            IsThrow = true;

            InnerHttpClient = new HttpClient(handler)
            {
                Timeout = timeout
            };
        }

        /// <summary>
        ///     创建用于网络请求的<see cref="ServiceClient" />实例
        /// </summary>
        /// <param name="httpClient"></param>
        public ServiceClient(HttpClient httpClient)
        {
            InnerHttpClient = httpClient;
        }

        #endregion

        #region [ Public Method ]

        /// <summary>Send an HTTP request as an asynchronous operation.</summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
        /// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return InnerHttpClient.SendAsync(request);
        }

        #endregion

        #region [ Protected ]

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
            switch (httpVerb)
            {
                case HttpVerb.Get:
                    result =
                        await
                            InnerHttpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
                    break;
                case HttpVerb.Post:
                    result =
                        await
                            InnerHttpClient.PostAsync(url, body, cancellationToken).ConfigureAwait(false);
                    break;
                case HttpVerb.Put:
                    result =
                        await InnerHttpClient.PutAsync(url, body, cancellationToken).ConfigureAwait(false);
                    break;
                case HttpVerb.Patch:
                    result = await PatchAsync(url, body, cancellationToken).ConfigureAwait(false);
                    break;
                case HttpVerb.Delete:
                    result =
                        await
                            InnerHttpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(httpVerb.ToString(), httpVerb, null);
            }

            return result;
        }

        #endregion

        #region [ Private Method ]
        private Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content,
            CancellationToken cancellationToken)
        {
            return InnerHttpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            }, cancellationToken);
        }

        #endregion

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose()
        {
            InnerHttpClient.Dispose();
        }
    }
}
