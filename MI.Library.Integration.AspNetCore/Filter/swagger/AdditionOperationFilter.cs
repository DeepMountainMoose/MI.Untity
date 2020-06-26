using MI.Common;
using MI.Library.Interface.Attribute;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MI.Library.Integration.AspNetCore.Filter.swagger
{
    public class AdditionOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context != null)
            {
                context.ApiDescription.TryGetMethodInfo(out var methodInfo);
                var attribute = methodInfo.GetCustomAttributes<ExtraSwaggerParameter>().FirstOrDefault();
                AddExtraParameters(operation, attribute?.ParameterNames);

                var actualReturnType = context.MethodInfo.ReturnType.Name == "Task`1"
                    ? context.MethodInfo.ReturnType.GenericTypeArguments.FirstOrDefault()
                    : context.MethodInfo.ReturnType;
                if (actualReturnType != null &&
                    !(actualReturnType.Name == "ApiResult`1" || actualReturnType.Name == "ApiResult"))
                {
                    operation?.Responses?.Remove("200");
                    operation?.Responses?.Add("200",
                        new Response()
                        {
                            Description = "Success",
                            Schema = actualReturnType == typeof(void) || actualReturnType == typeof(Task)
                                ? context.SchemaRegistry.GetOrRegister(typeof(ApiResult))
                                : context.SchemaRegistry.GetOrRegister(
                                    typeof(ApiResult<>).MakeGenericType(actualReturnType))
                        });
                    if (!(operation?.Responses?.ContainsKey("500") ?? false))
                        operation?.Responses?.Add("500", new Response()
                        {
                            Description = "Error: Internal Server Error",
                            Schema = context.SchemaRegistry.GetOrRegister(typeof(ApiResult))
                        });
                }
            }

            if (operation != null && operation.Parameters != null)
            {
                operation.Parameters.Add(new BodyParameter()
                {
                    Description = "操作人Id",
                    Name = "OperatorId",
                    Required = false,
                    In = "query",
                    Extensions = { ["type"] = "string" }
                });
                operation.Parameters.Add(new BodyParameter()
                {
                    Description = "平台",
                    Name = "Platform",
                    Required = true,
                    In = "query",
                    Extensions = { ["type"] = "string", ["enum"] = typeof(Platform).GetEnumNames() }
                });
                operation.Parameters.Add(new BodyParameter()
                {
                    Description = "是否追踪",
                    Name = "IsTrack",
                    Required = false,
                    In = "query",
                    Extensions = { ["type"] = "boolean" }
                });
            }
        }

        private void AddExtraParameters(Operation operation, string[] parameterNames)
        {
            if (parameterNames == null || parameterNames.Length == 0)
                return;

            foreach (var parameterName in parameterNames)
            {
                operation.Parameters.Add(new BodyParameter
                {
                    Name = parameterName,
                    Required = false,
                    In = "query",
                    Extensions = { ["type"] = "string" }
                });
            }
        }
    }
}
