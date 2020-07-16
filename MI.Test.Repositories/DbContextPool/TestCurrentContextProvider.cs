using MI.Core.Dependency;
using MI.Domain.Test.Interface.EntityFramework;
using MI.Library.Interface;
using MI.Library.Interface.Enum;
using MI.Test.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace MI.Test.Repositories.DbContextPool
{
    public class TestCurrentContextProvider : ITestCurrentContextProvider, ISingletonDependency
    {
        private readonly DbContextPool<TestContext> _dbContextPool;
        private readonly DbContextPool<TestContext> _dbReadContextPool;

        public TestCurrentContextProvider(IDbConnectionStringResolver dbConnectionStringResolver)
        {
            _dbContextPool = GetContextPool(dbConnectionStringResolver, DbConfigType.MI);
            _dbReadContextPool = GetContextPool(dbConnectionStringResolver, DbConfigType.MI, 30);
        }

        public ITestReadCurrentContext GetReadContext()
        {
            return new TestCurrentReadContext(new DbContextPool<TestContext>.Lease(_dbReadContextPool));
        }

        public ITestCurrentContext GetContext()
        {
            return new TestCurrentContext(new DbContextPool<TestContext>.Lease(_dbContextPool));
        }

        private DbContextPool<TestContext> GetContextPool(IDbConnectionStringResolver dbConnectionStringResolver, DbConfigType dbConfigType, int poolSize = 10)
        {
            var connectionString = dbConnectionStringResolver.ResolveConnectionString(dbConfigType);
            var builder = new DbContextOptionsBuilder<TestContext>();
            builder.UseSqlServer(connectionString, option => option.EnableRetryOnFailure());
            builder.EnableSensitiveDataLogging();
            builder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));

            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(builder.Options.FindExtension<CoreOptionsExtension>().WithMaxPoolSize(poolSize));
            return new DbContextPool<TestContext>(builder.Options);
        }
    }
}
