using MI.Domain.Test;
using MI.EntityFramework.Common.Reporitory;
using MI.EntityFrameworkCore.EntityFrameworkCore;
using MI.Test.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace MI.Test.Repositories.EntityFramework
{
    [RepositoryTypes(typeof(TestContextRepository<>), typeof(TestContextRepository<,>))]
    public class TestContext : FeIDbContext
    {
        public TestContext(DbContextOptions options)
       : base(options)
        {

        }

        public DbSet<SlideShowImg> SlideShowImg { get; set; }
    }
}
