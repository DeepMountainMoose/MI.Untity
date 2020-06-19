using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class DbContextManager<TDbContext> : IDbContextManager<TDbContext>
        where TDbContext:DbContextBase
    {
        private readonly string connectionString;
        private readonly string readOnlyConnectionString;
        private readonly IDbScope dbScope;
    }
}
