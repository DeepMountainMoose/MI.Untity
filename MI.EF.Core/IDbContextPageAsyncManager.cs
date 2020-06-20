using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public interface IDbContextPageAsyncManager<TDbContext> where TDbContext:DbContextBase
    {
        Task<IPageData<TEntity>> PageAsync<TEntity, TK1>(Func<TDbContext, IQueryable<TEntity>> query, int pageSize, int pageIndex, OrderByProperty<TEntity, TK1> orderBy1)
            where TEntity : class;

        Task<IPageData<TEntity>> PageAsync<TEntity>(Func<TDbContext, IQueryable<TEntity>> query, int pageSize, int pageNo)
            where TEntity : class;
    }
}
