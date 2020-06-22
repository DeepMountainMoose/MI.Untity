using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceClient
{
    /// <summary>
    ///     异常信息记录
    /// </summary>
    public class ExceptionData
    {
        /// <summary>
        ///     异常信息
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        ///     异常相关数据
        /// </summary>
        public Dictionary<string, string> Data { get; }

        /// <summary>
        ///     信息
        /// </summary>
        public string Message { get; set; }

        private ExceptionData()
        {
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        ///     记录确认状态时候的信息
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="url"></param>
        /// <param name="httpVerb"></param>
        /// <param name="contentString"></param>
        /// <returns></returns>
        public static ExceptionData LogEnsureSuccessed(Exception exception, string url, HttpVerb httpVerb, string contentString)
        {
            var ed = new ExceptionData { Exception = exception, Message = "确认Http状态码时异常" };
            ed.Data.Add("url", url);
            ed.Data.Add("httpverb", httpVerb.ToString());
            ed.Data.Add("type", "ensuresuccessed");
            if (!string.IsNullOrEmpty(contentString))
                ed.Data.Add("content", contentString);
            return ed;

        }

        /// <summary>
        ///     记录确认状态时候的信息
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="url"></param>
        /// <param name="httpVerb"></param>
        /// <param name="contentString"></param>
        /// <returns></returns>
        public static ExceptionData LogRequest(Exception exception, string url, HttpVerb httpVerb, string contentString)
        {
            var ed = new ExceptionData { Exception = exception, Message = "请求时候的未知异常" };
            ed.Data.Add("url", url);
            ed.Data.Add("httpverb", httpVerb.ToString());
            ed.Data.Add("type", "request");
            ed.Data.Add("errorType", "unknown");
            if (!string.IsNullOrEmpty(contentString))
                ed.Data.Add("content", contentString);
            return ed;

        }

        /// <summary>
        ///     记录确认状态时候的信息
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="url"></param>
        /// <param name="httpVerb"></param>
        /// <param name="contentString"></param>
        /// <returns></returns>
        public static ExceptionData LogRequestTimeout(Exception exception, string url, HttpVerb httpVerb, string contentString)
        {
            var ed = new ExceptionData { Exception = exception, Message = "请求时候的超时异常" };
            ed.Data.Add("url", url);
            ed.Data.Add("httpverb", httpVerb.ToString());
            ed.Data.Add("type", "request");
            ed.Data.Add("errorType", "timeout");
            if (!string.IsNullOrEmpty(contentString))
                ed.Data.Add("content", contentString);
            return ed;

        }
    }
}
