using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface
{
    /// <summary>
    ///     获取链接字符串
    /// </summary>
    public interface IDbConnectionStringResolver
    {
        /// <summary>
        ///     获取链接字符串
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        string ResolveConnectionString(DbConfigType configType);

        /// <summary>
        ///     获取链接字符串
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="parameter1"></param>
        /// <returns></returns>
        string ResolveConnectionString(DbConfigType configType, string parameter1);
    }
}
