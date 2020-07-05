#if NETSTANDARD2_1
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
namespace DapperWrapper
{
    public class SqlExecutorFactory : IDbExecutorFactory
    {
        public IDbExecutor CreateExecutor(string connectionString)
        {
            var dbConnection = new SqlConnection(connectionString);
            return new SqlExecutor(dbConnection);
        }
    }
}
