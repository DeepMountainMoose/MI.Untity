using MI.AspNetCore.AspNetCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MI.Library.Integration.AspNetCore.Options
{
    public class LibraryMiddlewareOptions
    {
        public bool EnableHttpRequestLog { get; set; } = true;

        public bool EnableSwaggerAuth { get; set; } = true;

        /// <summary>
        ///     是否使用压缩
        /// </summary>
        public bool EnableCompression { get; set; } = true;

        public DocExpansion SwaggerDocExpansion { get; set; } = DocExpansion.None;

        public Action<IRouteBuilder> RouteBuilderAction { get; set; }

        public Action<ApplicationBuilderOptions> FeIApplicationBuilderAction { get; set; }

        //public Action<ApplicationInsightsConfig> ApplicationInsightsConfigAction { get; set; }

        public List<CultureInfo> SupportedCultures { get; set; } = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("zh-CN")
        };

        public RequestCulture DefaultRequestCulture { get; set; } = new RequestCulture("zh-CN");
    }
}
