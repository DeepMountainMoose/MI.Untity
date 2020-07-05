using DapperWrapper;
using MI.Library.Interface.Enum;

namespace MI.Library.Interface
{
    /// <summary>
    ///     基于<see cref="DbConfigType"/>来获取<see cref="IDbExecutor"/>的接口定义
    /// </summary>
    public interface IDbExecutorFactoryWithDbConfigType
    {
        /// <summary>
        ///     通过<see cref="DbConfigType"/>获取对应的<see cref="IDbExecutor"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IDbExecutor CreateExecutor(DbConfigType type);

        /// <summary>
        ///     通过<see cref="DbConfigType"/>获取对应的<see cref="IDbExecutor"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paramater1"></param>
        /// <returns></returns>
        IDbExecutor CreateExecutor(DbConfigType type, string paramater1);
    }
}
