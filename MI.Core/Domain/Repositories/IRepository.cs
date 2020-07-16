using MI.Core.Dependency;
using MI.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MI.Core.Domain.Repositories
{
    /// <summary>
    ///     仓储基础接口,所有Repositories都必须实现此接口,用于标识仓储接口的定义.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    ///     一个 <see cref="IRepository{TEntity,TPrimaryKey}" /> 的实现.
    ///     <para>基于大部分主键都是<see cref="int" />的假设</para>
    /// </summary>
    /// <typeparam name="TEntity">
    ///     实体类型
    /// </typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }

    /// <summary>
    ///     所有Repositories都应该实现此接口以便实现其定义的基础仓储方法
    /// </summary>
    /// <typeparam name="TEntity">
    ///     仓储所对应的实体类型
    /// </typeparam>
    /// <typeparam name="TPrimaryKey">
    ///     仓储实体的主键
    /// </typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : ITransientDependency, IRepository
        where TEntity : class, IEntity<TPrimaryKey>
    {
        #region [ Query ]

        /// <summary>
        ///     用于获取用于从整个表中检索实体的 IQueryable.
        /// </summary>
        /// <remarks>
        ///     由于作用域范围关系请确保如果使用此方法是位于工作单元范围内
        ///     不然可能会产生数据上下文被Dispose的错误
        ///     请优先使用自身提供的查询方法
        /// </remarks>
        /// <returns>IQueryable 用于从数据库中检索实体</returns>
        IQueryable<TEntity> Query();

        /// <summary>
        ///     获取符合条件的实体
        /// </summary>
        /// <returns>满足条件的数据</returns>
        List<TEntity> QueryList(Expression<Func<TEntity, bool>> predicate);


        /// <summary>
        ///     获取符合条件的实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>满足条件的数据</returns>
        Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> predicate);


        /// <summary>
        ///     通过主键获取实体.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体的实例
        /// </returns>
        TEntity Find(TPrimaryKey id);

        /// <summary>
        ///     通过主键获取实体.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体的实例
        /// </returns>
        Task<TEntity> FindAsync(TPrimaryKey id);

        /// <summary>
        ///     通过给定的主键获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体实例或者null
        /// </returns>
        TEntity FirstOrDefault(TPrimaryKey id);

        /// <summary>
        ///     通过给定的主键获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="id">
        ///     实体的主键
        /// </param>
        /// <returns>
        ///     实体实例或者null
        /// </returns>
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        /// <summary>
        ///     通过给定的谓词搜索获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="predicate">
        /// </param>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     通过给定的谓词搜索获取实体,如果未找到则返回null.
        /// </summary>
        /// <param name="predicate">
        ///     过滤实体的谓词
        /// </param>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     通过给定的谓词搜索获取特定的一个实体,如果匹配的条件结果数目超过一个将异常.
        /// </summary>
        /// <param name="predicate">过滤实体的谓词</param>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     通过给定的谓词搜索获取特定的一个实体,如果匹配的条件结果数目超过一个将异常.
        /// </summary>
        /// <param name="predicate">过滤实体的谓词</param>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region [ Insert ]

        /// <summary>
        ///     插入一个新的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        TEntity Insert(TEntity entity);

        /// <summary>
        ///     插入一个新的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        ///     通过Id来插入或者更新实体.以Id为表示,如果该实体已经存在则更新实体.
        /// </summary>
        /// <param name="entity">
        ///     实体对象
        /// </param>
        TEntity InsertOrUpdate(TEntity entity);

        /// <summary>
        ///     通过Id来插入或者更新实体.以Id为表示,如果该实体已经存在则更新实体.
        /// </summary>
        /// <param name="entity">
        ///     实体对象
        /// </param>
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        /// <summary>
        ///     插入一个新的实体并返回其Id.此方法要求立即提交当前工作单元以便获得其Id.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        /// <returns>
        ///     实体的Id
        /// </returns>
        TPrimaryKey InsertAndGetId(TEntity entity);

        /// <summary>
        ///     插入一个新的实体并返回其Id.此方法要求立即提交当前工作单元以便获得其Id.
        /// </summary>
        /// <param name="entity">
        ///     需要被插入的实体
        /// </param>
        /// <returns>
        ///     实体的Id
        /// </returns>
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        #endregion

        #region [ Update ]

        /// <summary>
        ///     更新已存在的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被更新的实体对象
        /// </param>
        TEntity Update(TEntity entity);

        /// <summary>
        ///     更新已存在的实体.
        /// </summary>
        /// <param name="entity">
        ///     需要被更新的实体对象
        /// </param>
        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion

        #region [ Delete ]

        /// <summary>
        ///     删除一个实体.
        /// </summary>
        /// <param name="entity">
        ///     将要被删除的实体
        /// </param>
        void Delete(TEntity entity);

        /// <summary>
        ///     删除一个实体.
        /// </summary>
        /// <param name="entity">
        ///     将要被删除的实体
        /// </param>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        ///     通过主键删除一个实体.
        /// </summary>
        /// <param name="id">
        ///     将要被删除的实体Id
        /// </param>
        void Delete(TPrimaryKey id);

        /// <summary>
        ///     通过主键删除一个实体.
        /// </summary>
        /// <param name="id">
        ///     将要被删除的实体Id
        /// </param>
        Task DeleteAsync(TPrimaryKey id);

        /// <summary>
        ///     通过条件表达式删除满足条件的实体.
        ///     注: 备筛选的实体将会先返回获取到在执行删除,所以此方法可能会有性能上的问题
        /// </summary>
        /// <param name="predicate">
        ///     筛选实体的条件
        /// </param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     通过条件表达式删除满足条件的实体.
        ///     注: 备筛选的实体将会先返回获取到在执行删除,所以此方法可能会有性能上的问题
        /// </summary>
        /// <param name="predicate">
        ///     筛选实体的条件
        /// </param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region [ Count ]

        /// <summary>
        ///     返回当前存储库的所有实体总数.
        /// </summary>
        /// <returns>
        ///     实体总数
        /// </returns>
        int Count();

        /// <summary>
        ///     返回当前存储库的所有实体总数.
        /// </summary>
        /// <returns>
        ///     实体总数
        /// </returns>
        Task<int> CountAsync();

        /// <summary>
        ///     返回满足<paramref name="predicate" />的实体总数.
        /// </summary>
        /// <param name="predicate">
        ///     过滤总数筛选的方法
        /// </param>
        /// <returns>
        ///     实体总数
        /// </returns>
        int Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///     返回满足<paramref name="predicate" />的实体总数.
        /// </summary>
        /// <param name="predicate">
        ///     过滤总数筛选的方法
        /// </param>
        /// <returns>
        ///     实体总数
        /// </returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
