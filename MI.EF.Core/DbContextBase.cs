using MI.EF.Core.Interception;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MI.EF.Core
{
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
    }
}
