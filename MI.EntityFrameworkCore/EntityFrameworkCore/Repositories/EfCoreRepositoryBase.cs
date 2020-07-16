using MI.Core.Data;
using MI.Core.Domain.Entities;
using MI.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MI.EntityFrameworkCore.EntityFrameworkCore.Repositories
{
    public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public EfCoreRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext which contains <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class EfCoreRepositoryBase<TDbContext, TEntity, TPrimaryKey> :
        RepositoryBase<TEntity, TPrimaryKey>,
        IRepositoryWithDbContext

        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        public virtual TDbContext Context => _dbContextProvider.GetDbContext();

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public virtual DbTransaction Transaction
        {
            get
            {
                return (DbTransaction)TransactionProvider?.GetActiveTransaction(new ActiveTransactionProviderArgs
                {
                    {"ContextType", typeof(TDbContext) }
                });
            }
        }

        public virtual DbConnection Connection
        {
            get
            {
                var connection = Context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return connection;
            }
        }

        public IActiveTransactionProvider TransactionProvider { private get; set; }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfCoreRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        #region [ Query ]
        /// <summary>
        /// 用于获取用于从整个表中检索实体的 IQueryable。
        /// </summary>
        /// <returns>IQueryable 用于从数据库中检索实体</returns>
        public override IQueryable<TEntity> Query()
        {
            return Table;
        }

        public override List<TEntity> QueryList(Expression<Func<TEntity, bool>> predicate)
        {
            return Query().Where(predicate).ToList();
        }

        public override async Task<List<TEntity>> QueryListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Query().Where(predicate).ToListAsync();
        }
        #endregion

        #region [ Insert ]
        /// <summary>
        /// 插入一个新的实体.
        /// </summary>
        /// <param name="entity">需要被插入的实体</param>
        public override TEntity Insert(TEntity entity)
        {
            return Table.Add(entity).Entity;
        }

        /// <summary>
        /// 插入一个新的实体.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        /// <summary>
        /// 插入一个新的实体并返回其Id.
        /// <para>此方法要求立即提交当前工作单元以便获得其Id</para>
        /// </summary>
        /// <param name="entity">需要被插入的实体</param>
        /// <returns>实体的Id</returns>
        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);

            if (entity.IsTransient())
            {
                Context.SaveChanges();
            }

            return entity.Id;
        }

        /// <summary>
        /// 插入一个新的实体并返回其Id.
        /// <para>此方法要求立即提交当前工作单元以便获得其Id</para>
        /// </summary>
        /// <param name="entity">需要被插入的实体</param>
        /// <returns>实体的Id</returns>
        public override async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = await InsertAsync(entity);

            if (entity.IsTransient())
            {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        #endregion

        #region [ Update ]
        /// <summary>
        /// 更新实体.
        /// </summary>
        /// <param name="entity">需要被更新的实体对象</param>
        public override TEntity Update(TEntity entity)
        {
            Context.Update(entity);
            return entity;
        }

        /// <summary>
        /// 更新实体.
        /// </summary>
        /// <param name="entity">需要被更新的实体对象</param>
        public override Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity = Update(entity);
            return Task.FromResult(entity);
        }

        #endregion

        #region [ Delete ]
        /// <summary>
        /// 删除一个实体.
        /// </summary>
        /// <param name="entity">将要被删除的实体</param>
        public override void Delete(TEntity entity)
        {
            AttachIfNot(entity);

            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).IsDeleted = true;
            }
            else
            {
                Table.Remove(entity);
            }
        }

        /// <summary>
        /// 通过主键删除一个实体.
        /// </summary>
        /// <param name="id">将要被删除的实体的Id</param>
        public override void Delete(TPrimaryKey id)
        {
            var entity = GetFromChangeTrackerOrNull(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            entity = FirstOrDefault(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }
        }

        #endregion

        #region [ Count ]
        /// <summary>
        ///     返回当前存储库的所有实体总数
        /// </summary>
        /// <returns></returns>
        public override async Task<int> CountAsync()
        {
            return await Query().CountAsync();
        }

        /// <summary>
        ///     返回满足<paramref name="predicate" />的实体总数.
        /// </summary>
        /// <param name="predicate">
        ///     过滤总数筛选的方法
        /// </param>
        /// <returns>
        ///     实体总数
        /// </returns>
        public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Query().Where(predicate).CountAsync();
        }
        #endregion

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> propertyExpression,
            CancellationToken cancellationToken)
            where TProperty : class
        {
            return Context.Entry(entity).Collection(propertyExpression).LoadAsync(cancellationToken);
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellationToken)
            where TProperty : class
        {
            return Context.Entry(entity).Reference(propertyExpression).LoadAsync(cancellationToken);
        }

        private TEntity GetFromChangeTrackerOrNull(TPrimaryKey id)
        {
            var entry = Context.ChangeTracker.Entries()
                .FirstOrDefault(
                    ent =>
                        ent.Entity is TEntity &&
                        EqualityComparer<TPrimaryKey>.Default.Equals(id, (ent.Entity as TEntity).Id)
                );

            return entry?.Entity as TEntity;
        }
    }
}
