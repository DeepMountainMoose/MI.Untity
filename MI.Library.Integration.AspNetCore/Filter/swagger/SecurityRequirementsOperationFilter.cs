using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace MI.Library.Integration.AspNetCore.Filter.swagger
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var actionAttrs = context.MethodInfo.GetCustomAttributes(true);
            var controllerAttrs = context.MethodInfo.DeclaringType.GetCustomAttributes(true);

            if (actionAttrs.OfType<AllowAnonymousAttribute>().Any() || controllerAttrs.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var methodAuthorizeAttrs = actionAttrs.OfType<AuthorizeAttribute>();
            var controllerAuthorizeAttrs = controllerAttrs.OfType<AuthorizeAttribute>();

            if (!methodAuthorizeAttrs.Any() && !controllerAuthorizeAttrs.Any())
            {
                return;
            }

            operation.Responses.Add("401", new Response { Description = "Unauthorized" });

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    { "bearerAuth", new []{ string.Empty }  }
                }
            };
        }
    }
}
