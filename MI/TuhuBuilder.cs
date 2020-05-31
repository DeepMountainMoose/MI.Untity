using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI
{
    public class TuhuBuilder:ITuhuBuilder
    {
        public TuhuBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;

            Configuration = configuration;
        }

        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }
    }
}
