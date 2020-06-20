using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public interface IDbContextUpdateAsyncManager
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// 更新指定指定字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(TEntity entity, Action<TEntity> update)
            where TEntity : class;

        /// <summary>
        /// 更新指定数据字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkid"></param>
        /// <param name="updateProperty"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(int pkid, params IDifferUpdateProperty<TEntity>[] updateProperty)
            where TEntity : class;

        /// <summary>
        /// 批量更新指定数据的字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkids"></param>
        /// <param name="updateProperty"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(IEnumerable<int> pkids, params IDifferUpdateProperty<TEntity>[] updateProperty)
            where TEntity : class;

        /// <summary>
        /// 更新指定数据字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkid"></param>
        /// <param name="updateProperty"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(long pkid, params IDifferUpdateProperty<TEntity>[] updateProperty)
            where TEntity : class;

        /// <summary>
        /// 批量更新指定数据的字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkids"></param>
        /// <param name="updateProperty"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<TEntity>(IEnumerable<long> pkids, params IDifferUpdateProperty<TEntity>[] updateProperty)
            where TEntity : class;
    }
}
