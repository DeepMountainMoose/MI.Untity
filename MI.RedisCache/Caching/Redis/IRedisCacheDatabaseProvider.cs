using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.RedisCache.Caching.Redis
{
    /// <summary>
    ///     Used to get <see cref="IDatabase" /> for Redis cache.
    ///     <para>使用 <see cref="IDatabase" /> 提供Redis缓存</para>
    /// </summary>
    public interface IRedisCacheDatabaseProvider
    {
        /// <summary>
        ///     Gets the database connection.
        ///     <para>获取数据库连接</para>
        /// </summary>
        IDatabase GetDatabase();
    }
}
