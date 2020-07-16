using System.Data;

namespace MI.Core.Data
{
    public interface IActiveTransactionProvider
    {
        /// <summary>
        ///     获取当前活动的事务,如果当前工作单元是非事务性的那么则返回Null.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args);

        /// <summary>
        ///     获取当前活动的数据库连接.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args);
    }
}
