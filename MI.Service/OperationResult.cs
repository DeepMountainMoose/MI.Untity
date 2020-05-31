using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MI.Service
{
    /// <summary>静态类</summary>
    [DataContract]
    public abstract class OperationResult
    {
        [JsonProperty]
        public long ElapsedMilliseconds { get; set; }

        /// <summary>操作是否成功</summary>
        [DataMember]
        public bool Success { get; internal set; }

        /// <summary>错误代码，操作失败时才有的值</summary>
        [DataMember]
        public string ErrorCode { get; internal set; }

        /// <summary>错误消息，操作失败时才有的值</summary>
        [DataMember]
        public string ErrorMessage { get; internal set; }

        /// <summary>调用异常，操作失败时才有的值</summary>
        [IgnoreDataMember, JsonProperty]
        public Exception Exception { get; internal set; }

        /// <summary>ToString</summary>
        /// <returns>Json</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);

        /// <summary>将结果转换成OperationResult</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="result">结果</param>
        /// <returns>OperationResult</returns>
        public static OperationResult<T> FromResult<T>(T result) => new OperationResult<T> { Result = result, Success = true };

        /// <summary>将结果转换成OperationResult</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="result">结果</param>
        /// <returns>OperationResult</returns>
        public static async Task<OperationResult<T>> FromResultAsync<T>(Task<T> result) =>
            new OperationResult<T> { Result = await result.ConfigureAwait(false), Success = true };
//#if !NETFRAMEWORK
//        /// <summary>将结果转换成OperationResult</summary>
//        /// <typeparam name="T">类型</typeparam>
//        /// <param name="result">结果</param>
//        /// <returns>OperationResult</returns>
//        public static async ValueTask<OperationResult<T>> FromResultAsync<T>(ValueTask<T> result) =>
//            new OperationResult<T> { Result = await result.ConfigureAwait(false), Success = true };
//#endif
        /// <summary>将错误转换成OperationResult</summary>
        /// <param name="errorCode">错误代码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>OperationResult</returns>
        public static OperationResult FromError(string errorCode, string errorMessage) => new OperationResult<object> { ErrorCode = errorCode, ErrorMessage = errorMessage };

        /// <summary>将错误转换成OperationResult</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="errorCode">错误代码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>OperationResult</returns>
        public static OperationResult<T> FromError<T>(string errorCode, string errorMessage) => new OperationResult<T> { ErrorCode = errorCode, ErrorMessage = errorMessage };

        /// <summary>将结果转换成OperationResult</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="result">结果</param>
        /// <returns>OperationResult</returns>
        public static OperationResult<T> FromError<T>(OperationResult result) => new OperationResult<T> { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage, Exception = result.Exception };

        /// <summary>将异常转换成OperationResult</summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ex">异常</param>
        /// <returns>OperationResult</returns>
        internal static OperationResult<T> FromException<T>(Exception ex)
        {
            return new OperationResult<T>
            {
                Exception = ex,
                ErrorCode = ex.GetType().FullName,
                ErrorMessage = ex.Message
            };
        }

        #region ThrowIfException
        /// <summary>如果有错误则抛出异常</summary>
        public void ThrowIfException() => ThrowIfException(false);

        /// <summary>如果有错误则抛出异常</summary>
        /// <param name="throwError">是否包含Error，默认false</param>
        public void ThrowIfException(bool throwError)
        {
            if (Exception != null || throwError && ErrorCode != null)
                throw new TuhuSerivceException(this);
        }

        /// <summary>如果有错误则抛出异常</summary>
        /// <param name="message">错误消息</param>
        public void ThrowIfException(string message) => ThrowIfException(false, message);

        /// <summary>如果有错误则抛出异常</summary>
        /// <param name="throwError">是否包含Error，默认false</param>
        /// <param name="message">错误消息</param>
        public void ThrowIfException(bool throwError, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                ThrowIfException(throwError);
            else if (Exception != null || throwError && ErrorCode != null)
                throw new TuhuSerivceException(message, this);
        }
        #endregion
    }

    /// <summary>操作返回值</summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class OperationResult<T> : OperationResult
    {
        internal OperationResult() { }

        /// <summary>操作结果数据集</summary>
        [DataMember]
        public T Result { get; internal set; }

        internal static OperationResult<T> Default => FromResult(default(T));
    }

    public class RemoteException : Exception
    {
        public RemoteException() { }

        public RemoteException(string message, string exceptionType, string stackTrace, JObject innerException = null)
            : base(message ?? exceptionType, innerException == null ? null : Parse(innerException))
        {
            ExceptionType = exceptionType;
            StackTrace = stackTrace;
        }

        public string ExceptionType { get; }

        public override string StackTrace { get; }

        private class ExceptionJson
        {
            public string Message { get; set; }
            public string ExceptionMessage { set => Message = value; }

            public string ClassName { set => ExceptionType = value; }
            public string ExceptionType { get; set; }

            public string StackTraceString { set => StackTrace = value; }
            public string StackTrace { get; set; }

            public string Source { get; set; }

            public JObject InnerException { get; set; }
        }

        public static RemoteException Parse(JObject ex)
        {
            var json = ex?.ToObject<ExceptionJson>();
            if (json?.ExceptionType == null || json.StackTrace == null)
                return null;

            return new RemoteException(json.Message, json.ExceptionType, json.StackTrace, json.InnerException)
            {
                Source = json.Source
            };
        }

        public static RemoteException Parse(JsonReader reader) =>
            reader == null || reader.TokenType == JsonToken.Null ? null : Parse(JObject.Load(reader));
    }

    /// <inheritdoc />
    public class TuhuSerivceException : ApplicationException
    {
        /// <inheritdoc />
        internal TuhuSerivceException(string message, OperationResult result) : base($"{message}。ErrorCode: {result.ErrorCode}, ElapsedMilliseconds: {result.ElapsedMilliseconds}, ErrorMessage: {result.ErrorMessage}", result.Exception) { }

        /// <inheritdoc />
        internal TuhuSerivceException(OperationResult result) : base($"ErrorCode: {result.ErrorCode}, ElapsedMilliseconds: {result.ElapsedMilliseconds}, ErrorMessage: {result.ErrorMessage}", result.Exception) { }
    }

    public class ExceptionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Exception ex) WriteJson(writer, ex, serializer);
        }

        private static void WriteJson(JsonWriter writer, Exception ex, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(ex.Message));
            writer.WriteValue(ex.Message);
            writer.WritePropertyName(nameof(ex.StackTrace));
            writer.WriteValue(ex.StackTrace);
            writer.WritePropertyName(nameof(ex.Source));
            writer.WriteValue(ex.Source);
            writer.WritePropertyName(nameof(RemoteException.ExceptionType));
            writer.WriteValue(ex.GetType().FullName);

            if (ex.InnerException != null)
            {
                writer.WritePropertyName(nameof(ex.InnerException));
                WriteJson(writer, ex.InnerException, serializer);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            RemoteException.Parse(reader);

        public override bool CanConvert(Type objectType) => typeof(Exception).IsAssignableFrom(objectType);
    }
}
