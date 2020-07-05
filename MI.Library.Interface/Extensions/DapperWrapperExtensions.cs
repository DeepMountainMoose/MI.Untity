using DapperWrapper;
using MI.Library.Interface.Enum;

namespace MI.Library.Interface.Extensions
{
    /// <summary>
    ///     DapperWrapper扩展方法
    /// </summary>
    public static class DapperWrapperExtensions
    {
        private static IDbConnectionStringResolver _connectionStringResolver;

        /// <summary>
        ///     设置<see cref="IDbConnectionStringResolver"/>
        /// </summary>
        /// <param name="connectionStringResolver"></param>
        public static void SetDbConnectionStringResolver(IDbConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        /// <summary>
        ///     获取特定连接的<see cref="IDbExecutor"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbExecutor CreateExecutor(this IDbExecutorFactory factory, DbConfigType type)
        {
            var dbExecutorFactoryWithDbConfigType = factory as IDbExecutorFactoryWithDbConfigType;
            if (dbExecutorFactoryWithDbConfigType != null)
                return dbExecutorFactoryWithDbConfigType.CreateExecutor(type);

            if (_connectionStringResolver != null)
                return factory.CreateExecutor(
                    _connectionStringResolver.ResolveConnectionString(type));

            return null;

        }
    }
}
