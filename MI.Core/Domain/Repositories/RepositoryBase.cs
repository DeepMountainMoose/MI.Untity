using MI.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MI.Core.Domain.Repositories
{
    /// <summary>
    ///     <para><see cref="IRepository{TEntity,TPrimaryKey}" />的基础实现, 提供了仓储有关的基础方法实现</para>
    /// </summary>
    /// <typeparam name="TEntity">
    ///     此仓储的实体类型
    /// </typeparam>
    /// <typeparam name="TPrimaryKey">
    ///     实体的主键类型
    /// </typeparam>
    public abstract class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        ///     Id比较表达式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        #region [ Query ]

        /// <summary>
        ///     用于获取用于从整个表中检索实体的 IQueryable。
        /// </summary>
        /// <returns>IQueryable 用于从数据库中检索实体</returns>
        public abstract IQueryable<TEntity> Query();

        /// <summary>
        ///     获取符合条件的实体
        /// </summary>
        /// <returns>满足条件的数据</returns>
        public virtual List<TEntity> QueryList(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).ToList();
        }

        /// <summary>
        ///     获取符合条件的实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>满足条件的数据</returns>
        public virtual Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(QueryList(predicate));
        }

        /// <summary>
        ///     通过主键获取实体.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体的实例
        /// </returns>
        public virtual TEntity Find(TPrimaryKey id)
        {
            var entity = FirstOrDefault(id);
            return entity;
        }

        /// <summary>
        ///     通过主键获取实体
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体的实例
        /// </returns>
        public virtual async Task<TEntity> FindAsync(TPrimaryKey id)
        {
            var entity = await FirstOrDefaultAsync(id);
            return entity;
        }

        /// <summary>
        ///     通过给定的主键获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体实例或者null
        /// </returns>
        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Query().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        /// <summary>
        ///     通过给定的主键获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体实例或者null
        /// </returns>
        public virtual Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        /// <summary>
        ///     通过给定的谓词搜索获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="predicate">
        ///     过滤实体的谓词
        /// </param>
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().FirstOrDefault(predicate);
        }

        /// <summary>
        ///     通过给定的谓词搜索获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="predicate">
        ///     过滤实体的谓词
        /// </param>
        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        /// <summary>
        ///     通过给定的谓词搜索获取特定的一个实体,如果匹配的条件结果数目超过一个将异常.
        /// </summary>
        /// <param name="predicate">过滤实体的谓词</param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Single(predicate);
        }

        /// <summary>
        ///     通过给定的谓词搜索获取特定的一个实体,如果匹配的条件结果数目超过一个将异常.
        /// </summary>
        /// <param name="predicate">过滤实体的谓词</param>
        /// <returns></returns>
        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Single(predicate));
        }

        #endregion

        #region [ Insert ]

        /// <summary>
        ///     插入一个新的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        public abstract TEntity Insert(TEntity entity);

        /// <summary>
        ///     插入一个新的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        /// <summary>
        ///     通过Id来插入或者更新实体.以Id为表示,如果该实体已经存在则更新实体
        /// </summary>
        /// <param name="entity">
        ///     实体对象
        /// </param>
        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? Insert(entity)
                : Update(entity);
        }

        /// <summary>
        ///     通过Id来插入或者更新实体.以Id为表示,如果该实体已经存在则更新实体
        /// </summary>
        /// <param name="entity">
        ///     实体对象
        /// </param>
        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        /// <summary>
        ///     插入一个新的实体并返回其Id.此方法要求立即提交当前工作单元以便获得其Id
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        /// <returns>
        ///     实体的Id
        /// </returns>
        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            return Insert(entity).Id;
        }

        /// <summary>
        ///     插入一个新的实体并返回其Id.此方法要求立即提交当前工作单元以便获得其Id
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        /// <returns>
        ///     实体的Id
        /// </returns>
        public virtual Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertAndGetId(entity));
        }

        #endregion

        #region [ Update ]

        /// <summary>
        ///     更新已存在的实体
        /// </summary>
        /// <param name="entity">
        ///     需要被更新的实体对象
        /// </param>
        public abstract TEntity Update(TEntity entity);

        /// <summary>
        ///     更新已存在的实体
        /// </summary>
        /// <param name="entity">
        ///     需要被更新的实体对象
        /// </param>
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        #endregion

        #region [ Delete ]

        /// <summary>
        ///     删除一个实体
        /// </summary>
        /// <param name="entity">
        ///     将要被删除的实体
        /// </param>
        public abstract void Delete(TEntity entity);

        /// <summary>
        ///     删除一个实体
        /// </summary>
        /// <param name="entity">
        ///     将要被删除的实体
        /// </param>
        public virtual Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        /// <summary>
        ///     通过主键删除一个实体
        /// </summary>
        /// <param name="id">
        ///     将要被删除的实体Id
        /// </param>
        public abstract void Delete(TPrimaryKey id);

        /// <summary>
        ///     通过主键删除一个实体
        /// </summary>
        /// <param name="id">
        ///     将要被删除的实体Id
        /// </param>
        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
            return Task.FromResult(0);
        }

        /// <summary>
        ///     通过条件表达式删除满足条件的实体.
        ///     注: 备筛选的实体将会先返回获取到在执行删除,所以此方法可能会有性能上的问题
        /// </summary>
        /// <param name="predicate">
        ///     筛选实体的条件
        /// </param>
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in Query().Where(predicate).ToList())
                Delete(entity);
        }

        /// <summary>
        ///     通过条件表达式删除满足条件的实体.
        ///     注: 备筛选的实体将会先返回获取到在执行删除,所以此方法可能会有性能上的问题
        /// </summary>
        /// <param name="predicate">
        ///     筛选实体的条件
        /// </param>
        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
            return Task.FromResult(0);
        }

        #endregion

        #region [ Count ]

        /// <summary>
        ///     返回当前存储库的所有实体总数
        /// </summary>
        /// <returns>
        ///     实体总数
        /// </returns>
        public virtual int Count()
        {
            return Query().Count();
        }

        /// <summary>
        ///     返回当前存储库的所有实体总数
        /// </summary>
        /// <returns>
        ///     实体总数
        /// </returns>
        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        /// <summary>
        ///     返回满足<paramref name="predicate" />的实体总数
        /// </summary>
        /// <param name="predicate">
        ///     过滤总数筛选的方法
        /// </param>
        /// <returns>
        ///     实体总数
        /// </returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).Count();
        }

        /// <summary>
        ///     返回满足<paramref name="predicate" />的实体总数
        /// </summary>
        /// <param name="predicate">
        ///     过滤总数筛选的方法
        /// </param>
        /// <returns>
        ///     实体总数
        /// </returns>
        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        #endregion
    }
}
