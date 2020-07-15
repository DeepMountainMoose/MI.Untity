using Microsoft.EntityFrameworkCore;
namespace MI.EntityFrameworkCore.EntityFrameworkCore.Configuration
{
    public interface IDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        void Configure(DbContextConfiguration<TDbContext> configuration);
    }
}
