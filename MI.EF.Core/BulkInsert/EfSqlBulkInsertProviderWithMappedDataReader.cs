using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MI.EF.Core.BulkInsert
{
    public class EfSqlBulkInsertProviderWithMappedDataReader : ProviderBase<SqlConnection, SqlTransaction>
    {
        public override void Run<T>(IEnumerable<T> entities, SqlTransaction transaction)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                try
                {
                    var columnAttributeMapper = new ColumnAttributeMapper<T>();
                    DataTable dt = ColumnAttributeMapper<T>.MapToDataTable(entities);
                    bulkCopy.DestinationTableName = dt.TableName;
                    bulkCopy.BatchSize = 2000;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }
                    bulkCopy.WriteToServer(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        protected override SqlConnection CreateConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }
    }
}
