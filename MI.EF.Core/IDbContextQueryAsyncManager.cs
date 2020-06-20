using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public interface IDbContextQueryAsyncManager<TDbContext>
        where TDbContext:DbContextBase
    {
        Task<List<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;

        Task<List<TEntity>> QueryAsync<TEntity>(Func<TDbContext, IQueryable<TEntity>> query) where TEntity : class;

        Task<TEntity> SingleQueryAsync<TEntity>(Expression<Func<TEntity, bool>> where)
            where TEntity : class;

        Task<TEntity> SingleQueryAsync<TEntity, TOrderByKey>(Expression<Func<TEntity, bool>> where, OrderByProperty<TEntity, TOrderByKey> orderby)
            where TEntity : class;
    }
}
