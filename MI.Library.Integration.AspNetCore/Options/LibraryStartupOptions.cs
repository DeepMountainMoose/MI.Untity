using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace MI.Library.Integration.AspNetCore.Options
{
    public class LibraryStartupOptions
    {
        ///// <summary>
        /////     身份验证时是否返回401状态码. 默认返回
        ///// </summary>
        //public bool IsReturnUnauthorizedCode { get; set; } = true;

        ///// <summary>
        /////     是否禁用elc的JWT验证. 默认不禁用
        ///// </summary>
        //public bool DisableOldJwtAuthentication { get; set; } = false;

        /// <summary>
        ///     Mvc配置
        /// </summary>
        public Action<MvcOptions> MvcOptionsAction { get; set; }

        /// <summary>
        ///     Apollo配置
        /// </summary>
        public Action<IServiceCollection, IConfiguration> ApolloConfigureAction { get; set; }

        /// <summary>
        ///     Swagger配置
        /// </summary>
        public Action<SwaggerGenOptions> SwaggerGenOptionsAction { get; set; }

        /// <summary>
        ///     ServiceClient配置
        /// </summary>
        public Action<ServiceClientOptions> ServiceClientOptionsAction { get; set; }

        /// <summary>
        ///     健康检查配置
        /// </summary>
        public Action<IHealthChecksBuilder> HealthChecksBuilderAction { get; set; }
    }
}
