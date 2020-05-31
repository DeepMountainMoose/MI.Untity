using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI
{
    public interface ITuhuBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
