using MI.EF.Core.DbScope;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public partial class DbContextManager<TDbContext> : IDbContextManager<TDbContext>
        where TDbContext:DbContextBase
    {
        private readonly string connectionString;
        private readonly string readOnlyConnectionString;
        private readonly IDbScope dbScope;

        public DbContextManager(string connectionString)
            : this(connectionString, new DbRetryScope())
        { }

        public DbContextManager(string connectionString, IDbScope dbScope)
        {
            this.connectionString = connectionString;
            this.readOnlyConnectionString = connectionString;
            if(!this.readOnlyConnectionString.EndsWith(";",StringComparison.CurrentCultureIgnoreCase))
            {
                this.readOnlyConnectionString += ";";
            }
            this.readOnlyConnectionString += "ApplicationIntent=Readonly";
            this.dbScope = dbScope;

            DbContextPreloadManager.Preload<TDbContext>(connectionString);
        }


        public DbContextManager(string connectionString,string readOnlyConnectionString)
            :this(connectionString,readOnlyConnectionString,new DbRetryScope())
        { }

        public DbContextManager(string connectionString, string readOnlyConnectionString,IDbScope dbScope)
        {
            this.connectionString = connectionString;
            this.readOnlyConnectionString = readOnlyConnectionString;
            this.dbScope = dbScope;

            DbContextPreloadManager.Preload<TDbContext>(connectionString);
        }

        public IDbContextQueryAsyncManager<TDbContext> Primary => new DbContextManager<TDbContext>(this.connectionString, this.connectionString);
    }
}
