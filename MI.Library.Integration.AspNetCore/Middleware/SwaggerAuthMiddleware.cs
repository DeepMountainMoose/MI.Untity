using MI.Library.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.Library.Integration.AspNetCore.Middleware
{
    public class SwaggerAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var request = context?.Request;

            if (request == null)
                return _next(context);

            var path = request.Path;

            if (!path.HasValue || !path.Value.Contains("/swagger"))
                return _next(context);

            var host = request.Host;

            if (host.HasValue && (host.Value.Contains("127.0.0.1") || host.Value.Contains("::1") ||
                                  host.Value.Contains("localhost")))
                return _next(context);

            if (IsAuthenticated(request))
                return _next(context);


            //权限登录页面
            //context.Response.Redirect($"{ApplicationUrls.AdminLoginUrl}Account/Login?returnUrl={request.GetDisplayUrl()}");
            return Task.CompletedTask;
        }

        private static bool IsAuthenticated(HttpRequest request)
        {
            if (request?.Cookies == null)
                return false;

            if (!request.Cookies.TryGetValue(Constants.Authorization.CookieName, out var token))
                return false;

            if (string.IsNullOrEmpty(token))
                return false;

            return JsonWebTokenUtils.GetJwtPrincipal(token) != null;
        }
    }
}
