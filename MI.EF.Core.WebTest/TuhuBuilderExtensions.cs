using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.EF.Core.WebTest
{
    public static class TuhuBuilderExtensions
    {
        public static ITuhuBuilder AddServices(this ITuhuBuilder builder)
        {
            builder.Services.AddSingleton(EnvironmentHandler.Build(builder.Configuration));

            return builder;
        }
    }
}
