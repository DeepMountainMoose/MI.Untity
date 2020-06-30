using System;
using System.Web.Http;

namespace MI.Web.Api.Configuration
{
    /// <summary>
    ///     WebApi模块配置
    /// </summary>
    public interface IWebApiModuleConfiguration
    {
        /// <summary>
        /// 获取或者设置 <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }
    }
}
