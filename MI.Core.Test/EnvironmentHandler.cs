using MI.EF.Core.Env;
using MI.Library.Interface;
using MI.Library.Interface.Enum;

namespace MI.Core.Test
{
    public class EnvironmentHandler : EnvironmentHandlerBase<MIContext>, IEnvironmentHandler<MIContext>
    {
        private readonly IDbConnectionStringResolver dbConnectionStringResolver;
        public EnvironmentHandler(IDbConnectionStringResolver dbConnectionStringResolver)
        {
            this.dbConnectionStringResolver = dbConnectionStringResolver;
        }

        protected override string DBConnectionString => dbConnectionStringResolver.ResolveConnectionString(DbConfigType.MI);
        protected override string DBReadOnlyConnectionString => dbConnectionStringResolver.ResolveConnectionString(DbConfigType.MI);
    }
}
