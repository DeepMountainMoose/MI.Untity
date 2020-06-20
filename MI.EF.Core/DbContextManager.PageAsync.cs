using MI.Component.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public partial class DbContextManager<TDbContext> : IDbContextQueryAsyncManager<TDbContext>
        where TDbContext : DbContextBase
    {
        public async Task<IPageData<TEntity>> PageAsync<TEntity, TK1>(Func<TDbContext, IQueryable<TEntity>> query, int pageSize, int pageIndex,
             OrderByProperty<TEntity, TK1> orderBy1)
             where TEntity : class
        {
            if (pageSize <= 0)
            {
                throw new MIParameterException($"{nameof(pageSize)}必须大于0！");
            }
            if (pageIndex < 0)
            {
                throw new MIParameterException($"{nameof(pageIndex)}必须大于或等于0！");
            }

            return await this.InternalQueryAsync(async db =>
            {
                var result = query(db);
                bool ordered = false;
                if (orderBy1 != null)
                {
                    result = PropertyOrderBy(result, orderBy1, ref ordered);
                }

                return new PageData<TEntity>(await result.Skip(pageSize * pageIndex).Take(pageSize).ToListAsync(), result.Count());
            });
        }

        public async Task<IPageData<TEntity>> PageAsync<TEntity>(Func<TDbContext, IQueryable<TEntity>> query, int pageSize, int pageIndex)
            where TEntity : class
        {
            return await this.InternalQueryAsync(async db =>
            {
                var result = query(db);
                return new PageData<TEntity>(await result.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync(), result.Count());
            });
        }

        private IOrderedQueryable<TOrderByEntity> PropertyOrderBy<TOrderByEntity, TOrderByKey>(IQueryable<TOrderByEntity> query, OrderByProperty<TOrderByEntity, TOrderByKey> orderBy, ref bool ordered)
        {
            IOrderedQueryable<TOrderByEntity> orderedQuery = null;
            if (!ordered)
            {
                if (!orderBy.OrderByDesc)
                {
                    orderedQuery = query.OrderBy(orderBy.OrderByKey);
                }
                else
                {
                    orderedQuery = query.OrderByDescending(orderBy.OrderByKey);
                }
            }
            else
            {
                orderedQuery = query as IOrderedQueryable<TOrderByEntity>;
                if (!orderBy.OrderByDesc)
                {
                    orderedQuery = orderedQuery.ThenBy(orderBy.OrderByKey);
                }
                else
                {
                    orderedQuery = orderedQuery.ThenByDescending(orderBy.OrderByKey);
                }
            }
            ordered = true;
            return orderedQuery;
        }
    }
}
