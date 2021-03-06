﻿using System;
using MI.Library.Integration.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MI.Library.Integration.AspNetCore.Extensions;
using MI.EF.Core.OnConfiguring;

namespace MI.Core.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //EFCoreRegister.Use<SqlServerEFCore>();
            return services.AddEhi<SampleApplicationModule>(Common.Platform.MI, Library.Interface.StartupModeType.Api);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEhi();
        }
    }
}
