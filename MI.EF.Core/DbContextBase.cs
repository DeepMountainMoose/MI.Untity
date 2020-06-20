using MI.EF.Core.DbFunctions;
using MI.EF.Core.Interception;
using MI.EF.Core.OnConfiguring;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MI.EF.Core
{
    /// <summary>
    /// 继承自SqlServer 连接SqlServer数据库  
    /// </summary> 
    public partial class DbContextBase : DbContext
    {
        static DbContextBase()
        {
            DiagnosticListener.AllListeners.Subscribe(new DbCommandInterceptor());
        }

        private readonly string nameOrConnectionString;

        public DbContextBase(string nameOrConnectionString)
        {
            this.nameOrConnectionString = nameOrConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var context = ObjectContextFactory.Get();
            optionsBuilder = context.GetOptionsBuilder(optionsBuilder, nameOrConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddFunctions();
        }
    }
}
