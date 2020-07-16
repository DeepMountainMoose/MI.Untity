using MI.Domain.Test;
using MI.Domain.Test.Interface.EntityFramework;
using MI.Test.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;

namespace MI.Test.Repositories.DbContextPool
{
    public sealed class TestCurrentReadContext : ITestReadCurrentContext
    {
        private readonly DbContextPool<TestContext>.Lease _lease;
        private readonly TestContext _dbContext;

        public TestCurrentReadContext(DbContextPool<TestContext>.Lease lease)
        {
            _lease = lease;
            _dbContext = lease.Context;
        }

        public TestCurrentReadContext(TestContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SlideShowImg> SlideShowImg => _dbContext.SlideShowImg.AsNoTracking();

        public void Dispose()
        {
            ((IDisposable)_lease)?.Dispose();
        }
    }
}
