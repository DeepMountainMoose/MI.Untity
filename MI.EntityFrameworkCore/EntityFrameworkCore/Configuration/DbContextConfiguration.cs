using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace MI.EntityFrameworkCore.EntityFrameworkCore.Configuration
{
    public class DbContextConfiguration<TDbContext>
        where TDbContext : DbContext
    {
        public string ConnectionString { get; internal set; }

        public DbConnection ExistingConnection { get; internal set; }

        public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

        public DbContextConfiguration(string connectionString, DbConnection existingConnection)
        {
            ConnectionString = connectionString;
            ExistingConnection = existingConnection;

            DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        }
    }
}
