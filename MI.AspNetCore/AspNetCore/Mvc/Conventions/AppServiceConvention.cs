using Castle.Windsor.MsDependencyInjection;
using MI.AspNetCore.AspNetCore.Configuration;
using MI.Core;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.AspNetCore.AspNetCore.Mvc.Conventions
{
    public class AppServiceConvention : IApplicationModelConvention
    {
        private readonly Lazy<IAspNetCoreConfiguration> _configuration;

        public AppServiceConvention(IServiceCollection services)
        {
            _configuration = new Lazy<IAspNetCoreConfiguration>(() => services
                .GetSingletonService<Bootstrapper>()
                .IocManager
                .Resolve<IAspNetCoreConfiguration>(), true);
        }

        public void Apply(ApplicationModel application)
        {
            //nothing
        }
    }
}
