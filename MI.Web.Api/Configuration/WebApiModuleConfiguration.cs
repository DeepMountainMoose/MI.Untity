using System.Web.Http;

namespace MI.Web.Api.Configuration
{
    public class WebApiModuleConfiguration : IWebApiModuleConfiguration
    {
        public HttpConfiguration HttpConfiguration { get; set; }

        public WebApiModuleConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
        }
    }
}
