using MI.Component.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public partial class DbContextManager<TDbContext> : IDbContextQueryAsyncManager<TDbContext>
        where TDbContext : DbContextBase
    {
        public async Task<List<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> where)
            where TEntity : class
        {
            if (where == null)
            {
                where = _ => true;
            }
            return await this.InternalQueryAsync(async db => await db.Set<TEntity>().Where(where).ToListAsync());
        }

        public async Task<List<TEntity>> QueryAsync<TEntity>(Func<TDbContext, IQueryable<TEntity>> query)
            where TEntity : class
        {
            ParameterChecker.CheckNull(query, nameof(query));

            return await this.InternalQueryAsync(async db => await query(db).ToListAsync());
        }

        public async Task<TEntity> SingleQueryAsync<TEntity>(Expression<Func<TEntity, bool>> where)
            where TEntity : class
        {
            if (where == null)
            {
                where = _ => true;
            }

            return await this.InternalQueryAsync(async db => await db.Set<TEntity>().FirstOrDefaultAsync(where));
        }

        public async Task<TEntity> SingleQueryAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, OrderByProperty<TEntity, TKey> orderby)
            where TEntity : class
        {
            ParameterChecker.CheckNull(orderby, nameof(orderby));

            if (where == null)
            {
                where = _ => true;
            }


            return await this.InternalQueryAsync(async db =>
            {
                var query = db.Set<TEntity>().Where(where);
                if (orderby.OrderByDesc)
                {
                    query = query.OrderByDescending(orderby.OrderByKey);
                }
                else
                {
                    query = query.OrderBy(orderby.OrderByKey);
                }

                return await query.FirstOrDefaultAsync();
            });
        }

        public async Task<TEntity> SingleQueryAsync<TEntity>(Func<TDbContext, IQueryable<TEntity>> query)
        {
            ParameterChecker.CheckNull(query, nameof(query));

            return await this.InternalQueryAsync(async db => await query(db).FirstOrDefaultAsync());
        }

        public async Task<TEntity> SingleQueryAsync<TEntity, TOrderByKey>(Func<TDbContext, IQueryable<TEntity>> query, OrderByProperty<TEntity, TOrderByKey> orderby)
        {
            ParameterChecker.CheckNull(query, nameof(query));
            ParameterChecker.CheckNull(orderby, nameof(orderby));

            return await this.InternalQueryAsync(async db =>
            {
                var result = query(db);
                if(orderby.OrderByDesc)
                {
                    result = result.OrderByDescending(orderby.OrderByKey);
                }
                else
                {
                    result = result.OrderBy(orderby.OrderByKey);
                }

                return await result.FirstOrDefaultAsync();
            });
        }

        public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity,bool>> where)
            where TEntity:class
        {
            if(where==null)
            {
                where = (_ => true);
            }

            return await this.InternalQueryAsync(async db => await db.Set<TEntity>().Where(where).CountAsync());
        }

        public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity,bool>> where)
            where TEntity:class
        {
            if(where==null)
            {
                where = _ => true;
            }

            return await this.InternalQueryAsync(async db => await db.Set<TEntity>().AnyAsync(where));
        }

        private async Task<TResult> InternalQueryAsync<TResult>(Func<TDbContext, Task<TResult>> query)
        {
            ParameterChecker.CheckNull(query, "query");
            var db = (TDbContext)Activator.CreateInstance(typeof(TDbContext), this.readOnlyConnectionString);
            db.ChangeTracker.AutoDetectChangesEnabled = false;
            using (db)
            {
                return await this.dbScope.ExecuteAsync(() => query(db));
            }
        }
    }
}
