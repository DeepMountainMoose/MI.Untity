using MI.Component.Core.Exceptions;
using MI.EF.Core.Env;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.EF.Core.WebTest
{
    public class EnvironmentHandler : EnvironmentHandlerBase<MIContext>, IEnvironmentHandler<MIContext>
    {
        private static IEnvironmentHandler<MIContext> env = null;

        public static IEnvironmentHandler<MIContext> Build(IConfiguration configuration)
        {
            env = new EnvironmentHandler(configuration);

            return env;
        }

        public static IEnvironmentHandler<MIContext> Instance
        {
            get
            {
                if (env == null)
                {
                    throw new Exception("EnvironmentHandler未初始化，访问Instance之前需要调用Build方法");
                }

                return env;
            }
        }

        private readonly IConfiguration configuration;
        private EnvironmentHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override string DBConnectionString => this.configuration.GetConnectionString("SqlConnection");
        protected override string DBReadOnlyConnectionString => this.configuration.GetConnectionString("SqlConnection");
        //protected override string ApplicationName => "MI.Untity";
    }
}
