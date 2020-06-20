using MI.EF.Core.BulkInsert;
using MI.EF.Core.OnConfiguring;
using Microsoft.EntityFrameworkCore;
using System;

namespace MI.EF.Core
{
    public class DbContextPreloadManager
    {
        private static bool isInitialized = false;
        private static readonly object initializeLock = new object();

        public static void Preload<TDbContext>(string nameOrConnstringString)
            where TDbContext:DbContextBase
        {
            if (isInitialized)
                return;

            lock(initializeLock)
            {
                if(isInitialized)
                {
                    return;
                }

                var db = !string.IsNullOrEmpty(nameOrConnstringString) ?
                    (TDbContext)Activator.CreateInstance(typeof(TDbContext), nameOrConnstringString) :
                    (TDbContext)Activator.CreateInstance(typeof(TDbContext));

                ProviderFactory.Register<EfSqlBulkInsertProviderWithMappedDataReader>("System.Data.SqlClient.SqlConnection");
                if(db.Database.IsSqlServer())
                {
                    ObjectContextFactory.Register<MsSqlOptionsBuilder>("System.Data.SqlClient");
                }
                isInitialized = true;
            }
        }
    }


    public class EfProfilingSqlBulkInsertProviderWithMappedDataReader : EfSqlBulkInsertProviderWithMappedDataReader
    {
        protected override string ConnectionString => this.DbConnection.ConnectionString;
    }
}
