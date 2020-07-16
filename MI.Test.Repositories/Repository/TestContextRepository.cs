using MI.Core.Domain.Entities;
using MI.EntityFrameworkCore.EntityFrameworkCore;
using MI.EntityFrameworkCore.EntityFrameworkCore.Repositories;
using MI.Test.Repositories.EntityFramework;

namespace MI.Test.Repositories.Repository
{
    public class TestContextRepository<TEntity, TPrimaryKey> : EfCoreRepositoryBase<TestContext, TEntity, TPrimaryKey>
            where TEntity : class, IEntity<TPrimaryKey>
    {
        public TestContextRepository(IDbContextProvider<TestContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    public class TestContextRepository<TEntity> : EfCoreRepositoryBase<TestContext, TEntity>
        where TEntity : class, IEntity
    {
        public TestContextRepository(IDbContextProvider<TestContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
