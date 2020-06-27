using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI.Library.Integration.AspNetCore.Middleware
{
    /// <summary>
    ///     请求日志记录中间件
    /// </summary>
    public class HttpRequestLogMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ObjectPool<StringBuilder> _stringBuilderPool;

        private int _isRunning;

        private const char Split = '\t';

        private bool IsRunning => _isRunning == 1;

#if AEGIS
        public HttpRequestLogMiddleware(RequestDelegate next, ObjectPool<StringBuilder> stringBuilderPool, IHostApplicationLifetime lifetime)
#else
        public HttpRequestLogMiddleware(RequestDelegate next, ObjectPool<StringBuilder> stringBuilderPool, IApplicationLifetime lifetime)
#endif
        {
            _next = next;
            _stringBuilderPool = stringBuilderPool;
            _isRunning = 0;

            RegisterApplicationLifetime(lifetime);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsRunning)
            {
                await _next(context);
                return;
            }

            var watch = Stopwatch.StartNew();

            var request = context.Request;
            var connection = context.Connection;
            var response = context.Response;

            await _next(context);

            watch.Stop();

            var sb = _stringBuilderPool.Get();

            try
            {
                var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sb.Append(time).Append(Split); // timestamp

                var host = connection.LocalIpAddress.ToString();
                sb.Append(host).Append(Split); // host ip

                var method = request.Method;
                sb.Append(method).Append(Split); // method

                var path = request.Path.HasValue ? request.Path.Value : "-";
                sb.Append(path).Append(Split); // path

                var query = request.QueryString.HasValue ? request.QueryString.Value : "-";
                sb.Append(query).Append(Split); // query

                var port = request.Host.Port.HasValue ? request.Host.Port.Value.ToString() : "-";
                sb.Append(port).Append(Split); // port

                sb.Append("-").Append(Split); // username

                var clientIp = GetClientIp(context);
                sb.Append(clientIp).Append(Split); // client ip

                var ua = request.Headers.ContainsKey("User-Agent") ? request.Headers["User-Agent"].FirstOrDefault() : "-";
                sb.Append(ua).Append(Split); // useragent

                var refer = request.Headers.ContainsKey("Referer") ? request.Headers["Referer"].FirstOrDefault() : "-";
                sb.Append(refer).Append(Split); // refer

                sb.Append(response.StatusCode).Append(Split); // response

                var responseContentLength = response.ContentLength.HasValue ? response.ContentLength.Value.ToString() : "-";
                sb.Append(responseContentLength).Append(Split); // sc-bytes

                var requestContentLength = request.ContentLength.HasValue ? request.ContentLength.Value.ToString() : "-";
                sb.Append(requestContentLength).Append(Split); // cs-bytes

                var run = watch.ElapsedMilliseconds.ToString();
                sb.Append(run); // time taken 

                var logger = LoggerFactory.Create();
                logger.Information(sb.ToString());
            }
            finally
            {
                _stringBuilderPool.Return(sb);
            }
        }

        private void Open()
        {
            Interlocked.Exchange(ref _isRunning, 1);
        }

        private void Close()
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }

#if AEGIS
        private void RegisterApplicationLifetime(IHostApplicationLifetime applicationLifetime)
#else
        private void RegisterApplicationLifetime(IApplicationLifetime applicationLifetime)
#endif
        {
            applicationLifetime.ApplicationStarted.Register(Open);
            applicationLifetime.ApplicationStopped.Register(Close);
        }

        private static string GetClientIp(HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            return ip;
        }

        private static class LoggerFactory
        {
            private static readonly ConcurrentDictionary<DateTime, ILogger> LoggerCache = new ConcurrentDictionary<DateTime, ILogger>();

            public static ILogger Create()
            {
                var today = DateTime.Today;

                if (LoggerCache.TryGetValue(today, out var value))
                {
                    return value;
                }

                LoggerCache.Clear();

                var logger = new LoggerConfiguration()
                    .WriteTo.File(Path.Combine("log", $"req-{today:yyyy-MM-dd}.log"),
                        fileSizeLimitBytes: null,
                        outputTemplate: "{Message}{NewLine}",
                        buffered: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .CreateLogger();

                LoggerCache.TryAdd(today, logger);

                return logger;
            }
        }
    }
}
