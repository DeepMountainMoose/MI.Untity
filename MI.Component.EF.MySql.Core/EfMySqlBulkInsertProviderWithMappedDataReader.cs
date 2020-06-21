using MI.EF.Core.BulkInsert;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Component.EF.MySql.Core
{
    public class EfMySqlBulkInsertProviderWithMappedDataReader: ProviderBase<MySqlConnection, MySqlTransaction>
    {
        protected override string ConnectionString
        {
            get {
                return base.DbConnection.ConnectionString;
            }
        }

        public override void Run<T>(IEnumerable<T> entities, MySqlTransaction transaction)
        {
            var efMySqlBulkInsert = new EfMysqlBulkInsert();
            efMySqlBulkInsert.BulkInsertSql(entities, transaction);
        }

        protected override MySqlConnection CreateConnection()
        {
            return new MySqlConnection(this.ConnectionString);
        }
    }

    public class EfProfilingMySqlBulkInsertProviderWithMappedDataReader : EfMySqlBulkInsertProviderWithMappedDataReader
    {
        protected override string ConnectionString => this.DbConnection.ConnectionString;
    }
}
