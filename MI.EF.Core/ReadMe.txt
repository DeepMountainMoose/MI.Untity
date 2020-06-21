1. 项目直接Nuget引用MI.EF.Core
2. 项目里新建xxContext上下文  继承DBContextBase 内容如下：

    public partial class MIContext : DbContextBase
    {
        public MIContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }


3. 项目新建EnvironmentHandler类文件

    public class EnvironmentHandler : EnvironmentHandlerBase<MIContext>, IEnvironmentHandler<MIContext>
    {
        private static IEnvironmentHandler<MIContext> env = null;

        public static IEnvironmentHandler<MIContext> Build(IConfiguration configuration)
        {
            env = new EnvironmentHandler(configuration);

            return env;
        }

        /// <summary>
        /// 用于无法依赖注入的地方
        /// </summary>
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

4. 然后在需要用的地方使用如下代码：

 private readonly IEnvironmentHandler<MIContext> env;
 private readonly IServiceProvider serviceProvider;

 public ValuesController(IServiceProvider serviceProvider)
 {
     this.serviceProvider = serviceProvider;
     this.env = serviceProvider.GetRequiredService<IEnvironmentHandler<MIContext>>();
 }