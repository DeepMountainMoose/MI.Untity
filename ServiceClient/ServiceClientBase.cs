using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using ServiceClient.ObjectPolicy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceClient
{
    /// <summary>
    ///     提供<see cref="IServiceClient"/>的基础实现
    /// </summary>
    public abstract class ServiceClientBase : IServiceClient
    {
        static ServiceClientBase()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            JsonSerializerPool = new DefaultObjectPool<JsonSerializer>(new JsonSerializerObjectPolicy(JsonSerializerSettings));
        }

        private static readonly ObjectPool<StringBuilder> InnerStringBuilderPool =
            new DefaultObjectPoolProvider().CreateStringBuilderPool();

        private static readonly ObjectPool<JsonSerializer> JsonSerializerPool;

        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> PropertiesCache =
            new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        #region [ Property ]

        /// <summary>
        ///     异常日志记录委托
        /// </summary>
        public static Action<ExceptionData> ExceptionLogger { get; set; }

        /// <summary>
        ///     Json序列化设置
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings { get; }

        /// <summary>
        ///     默认请求头
        /// </summary>
        public abstract HttpRequestHeaders DefaultRequestHeaders { get; }

        /// <summary>
        ///     超时时间
        /// </summary>
        public abstract TimeSpan Timeout { get; }

        /// <summary>
        ///     当Http返回的请求状态码大于400的时候是否抛出异常
        ///     Default:true
        /// </summary>
        public bool IsThrow { get; set; }

        #endregion

        #region [ Public Method ]

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <returns>结果反序列化为<typeparamref name="T"/>后返回</returns>
        public Task<T> RequestAsync<T>(string url, HttpVerb method)
        {
            return RequestAsync<T>(url, method, null);
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <returns>结果反序列化为<typeparamref name="T"/>后返回</returns>
        public Task<T> RequestAsync<T>(string url, HttpVerb method, object requestObj)
        {
            return RequestAsync<T>(url, method, requestObj, CancellationToken.None);
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <returns>返回字符串表示的结果</returns>
        public Task<string> RequestAsync(string url, HttpVerb method)
        {
            return RequestAsync(url, method, null, CancellationToken.None);
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <returns>返回字符串表示的结果</returns>
        public Task<string> RequestAsync(string url, HttpVerb method, object requestObj)
        {
            return RequestAsync(url, method, requestObj, CancellationToken.None);
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="content">自定义请求Http信息</param>
        /// <returns>结果反序列化为<typeparamref name="T"/>后返回</returns>
        public Task<T> RequestAsync<T>(string url, HttpVerb method, HttpContent content)
        {
            return RequestAsync<T>(url, method, content, CancellationToken.None);
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="content">自定义请求Http信息</param>
        /// <returns>返回字符串表示的结果</returns>
        public Task<string> RequestAsync(string url, HttpVerb method, HttpContent content)
        {
            return RequestAsync(url, method, content, CancellationToken.None);
        }


        /// <summary>
        ///     透过<see cref="HttpRequestMessage"/>进行原始的Http请求并获取未处理的<see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string url, HttpVerb method, object requestObj,
            CancellationToken cancellationToken)
        {
            var response =
                await RequestInternalAsync(url, method, requestObj, cancellationToken, JsonSerializerSettings).ConfigureAwait(false)
                    ;
            return DeserializeFromStream<T>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <param name="jsonSerializerSettings">Json序列化配置</param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string url, HttpVerb method, object requestObj,
            JsonSerializerSettings jsonSerializerSettings)
        {
            var response =
                await RequestInternalAsync(url, method, requestObj, CancellationToken.None, jsonSerializerSettings).ConfigureAwait(false)
                    ;
            return DeserializeFromStream<T>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> RequestAsync(string url, HttpVerb method, object requestObj,
            CancellationToken cancellationToken)
        {
            var response =
                await RequestInternalAsync(url, method, requestObj, cancellationToken, JsonSerializerSettings).ConfigureAwait(false)
                    ;
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj">请求参数</param>
        /// <param name="jsonSerializerSettings">Json序列化配置</param>
        /// <returns></returns>
        public async Task<string> RequestAsync(string url, HttpVerb method, object requestObj,
            JsonSerializerSettings jsonSerializerSettings)
        {
            var response =
                await RequestInternalAsync(url, method, requestObj, CancellationToken.None, jsonSerializerSettings).ConfigureAwait(false)
                    ;
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="content">自定义请求Http信息</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string url, HttpVerb method, HttpContent content,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await SendRequest(url, method, content, cancellationToken).ConfigureAwait(false);
                return DeserializeFromStream<T>(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
            }
            finally
            {
                content?.Dispose();
            }
        }

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="content">自定义请求Http信息</param>
        /// <param name="cancellationToken">请求取消令牌</param>
        /// <returns>返回字符串表示的结果</returns>
        public async Task<string> RequestAsync(string url, HttpVerb method, HttpContent content,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await SendRequest(url, method, content, cancellationToken).ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            finally
            {
                content?.Dispose();
            }
        }

        #endregion

        #region [ Protected ]

        /// <summary>
        ///     通过Http请求数据
        /// </summary>
        /// <param name="url">请求地址Url</param>
        /// <param name="method">Http请求谓词</param>
        /// <param name="requestObj"></param>
        /// <param name="cancellationToken">请求取消令牌</param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns>返回字符串表示的结果</returns>
        protected Task<HttpResponseMessage> RequestInternalAsync(string url, HttpVerb method,
            object requestObj, CancellationToken cancellationToken, JsonSerializerSettings jsonSerializerSettings)
        {
            return SendRequest(FormatUrl(url, requestObj, method), method, FormatParameter(requestObj, method, jsonSerializerSettings), cancellationToken);
        }

        /// <summary>
        ///     内部发出请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpVerb"></param>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task<HttpResponseMessage> GetHttpResponseMessage(string url, HttpVerb httpVerb,
            HttpContent body, CancellationToken cancellationToken);

        /// <summary>
        ///     格式化Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestObj"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected internal static string FormatUrl(string url, object requestObj, HttpVerb method)
        {
            if (method == HttpVerb.Get || method == HttpVerb.Delete)
            {
                if (requestObj == null)
                    return url;

                var type = requestObj.GetType();

                IEnumerable<PropertyInfo> props;
                if (!PropertiesCache.ContainsKey(type))
                {
                    props = requestObj.GetType().GetRuntimeProperties();
                    PropertiesCache.TryAdd(type, props);
                }
                else
                {
                    props = PropertiesCache[type];
                }

                if (!props.Any())
                    return url;

                var paramater = GetNameValueCollectionString(props.Select(x => new KeyValuePair<string, string>(
                    x.Name, x.GetValue(requestObj) == null ? string.Empty : x.GetValue(requestObj) is DateTime time ? time.ToString("O") : x.GetValue(requestObj).ToString())).ToList());

                return url.Contains("?")
                    ? $"{url}&{paramater}"
                    : $"{url}?{paramater}";
            }

            return url;
        }

        /// <summary>
        ///     格式化参数
        /// </summary>
        /// <param name="requestObj"></param>
        /// <param name="method"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        protected static HttpContent FormatParameter(object requestObj, HttpVerb method,
            JsonSerializerSettings jsonSerializerSettings)
        {
            HttpContent body;
            if (method != HttpVerb.Get && method != HttpVerb.Delete)
            {
                body = requestObj != null
                    ? CreateHttpContent(requestObj, jsonSerializerSettings)
                    : new StringContent(string.Empty);
            }
            else
            {
                body = null;
            }

            return body;
        }
        #endregion

        #region [ Private Method ]

        /// <summary>
        ///     根据对象构造HttpContent
        /// </summary>
        /// <param name="content"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static HttpContent CreateHttpContent(object content, JsonSerializerSettings settings)
        {
            var serializer = ReferenceEquals(settings, JsonSerializerSettings) ? JsonSerializerPool.Get() : JsonSerializer.Create(settings);
            return new JsonContent(content, serializer);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        private static T DeserializeFromStream<T>(Stream jsonStream)
        {
            if (jsonStream == null)
                throw new ArgumentNullException(nameof(jsonStream));

            if (!jsonStream.CanRead)
                throw new ArgumentException("Json Stream must support reading", nameof(jsonStream));

            T deserializedObj;
            using (var sr = new StreamReader(jsonStream))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    var serializer = JsonSerializerPool.Get();
                    try
                    {
                        deserializedObj = serializer.Deserialize<T>(reader);
                    }
                    finally
                    {
                        JsonSerializerPool.Return(serializer);
                    }

                }
            }

            return deserializedObj;
        }


        private async Task<HttpResponseMessage> SendRequest(string url, HttpVerb httpVerb, HttpContent body,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage result;
            try
            {
                result = await GetHttpResponseMessage(url, httpVerb, body, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException te)
            {
                ExceptionData.LogRequestTimeout(te, url, httpVerb,
                    await GetSafeContentString(body).ConfigureAwait(false));
                throw;
            }
            catch (Exception e)
            {
                ExceptionLogger?.Invoke(ExceptionData.LogRequest(e, url, httpVerb,
                    await GetSafeContentString(body).ConfigureAwait(false)));
                throw;
            }

            if (!IsThrow)
                return result;

            if (!result.IsSuccessStatusCode)
            {
                var e = new HttpRequestException(string.Format(CultureInfo.InvariantCulture,
                    "Response status code does not indicate success: {0} ({1}).",
                    result.StatusCode,
                    result.ReasonPhrase
                ));
                ExceptionLogger?.Invoke(ExceptionData.LogEnsureSuccessed(e, url, httpVerb,
                    await GetSafeContentString(body)));

                throw e;
            }

            return result;
        }

        private async Task<string> GetSafeContentString(HttpContent content)
        {
            string contentString = string.Empty;
            try
            {
                contentString = await content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
                //ignore
            }

            return contentString;
        }

        private static string GetNameValueCollectionString(
            IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
                return string.Empty;
            var stringBuilder = InnerStringBuilderPool.Get();
            try
            {
                foreach (var nameValue in nameValueCollection)
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append('&');
                    stringBuilder.Append(Encode(nameValue.Key));
                    stringBuilder.Append('=');
                    stringBuilder.Append(Encode(nameValue.Value));
                }

                return stringBuilder.ToString();
            }
            finally
            {
                InnerStringBuilderPool.Return(stringBuilder);
            }
        }


        private static string Encode(string data)
        {
            return string.IsNullOrEmpty(data) ? string.Empty : Uri.EscapeDataString(data).Replace("%20", "+");
        }

        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public abstract void Dispose();
    }
}
