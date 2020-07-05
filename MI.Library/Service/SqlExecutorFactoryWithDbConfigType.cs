using DapperWrapper;
using MI.Library.Interface;
using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Service
{
    /// <summary>
    ///     基于<see cref="DbConfigType"/>进行扩展实现的<see cref="SqlExecutorFactory"/>
    /// </summary>
    public class SqlExecutorFactoryWithDbConfigType : SqlExecutorFactory, IDbExecutorFactoryWithDbConfigType
    {
        private readonly IDbConnectionStringResolver _connectionStringResolver;

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="connectionStringResolver"></param>
        public SqlExecutorFactoryWithDbConfigType(IDbConnectionStringResolver connectionStringResolver)
        {
            _connectionStringResolver = connectionStringResolver;
        }

        /// <summary>
        ///     通过<see cref="DbConfigType"/>获取对应的<see cref="IDbExecutor"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IDbExecutor CreateExecutor(DbConfigType type)
        {
            return CreateExecutor(
                _connectionStringResolver.ResolveConnectionString(type));
        }

        /// <summary>
        ///     通过<see cref="DbConfigType"/>获取对应的<see cref="IDbExecutor"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paramater1"></param>
        /// <returns></returns>
        public IDbExecutor CreateExecutor(DbConfigType type, string paramater1)
        {
            return CreateExecutor(
                _connectionStringResolver.ResolveConnectionString(type, paramater1));
        }
    }
}
