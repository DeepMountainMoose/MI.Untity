using MI.Domain.Test;
using MI.Domain.Test.Interface.EntityFramework;
using MI.Test.Repositories.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Test.Repositories.DbContextPool
{
    public class TestCurrentContext : ITestCurrentContext
    {
        private readonly DbContextPool<TestContext>.Lease _lease;
        private readonly TestContext _dbContext;
        private bool _isError;

        public TestCurrentContext(DbContextPool<TestContext>.Lease lease)
        {
            _lease = lease;
            _dbContext = lease.Context;
        }

        public TestCurrentContext(TestContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SlideShowImg> SlideShowImg => _dbContext.SlideShowImg;

        public void BulkInsert<TEntity>(IList<TEntity> entities)
            where TEntity : class
        {
            try
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                _dbContext.AddRange(entities);
                _dbContext.SaveChanges();
            }
            catch
            {
                _isError = true;
                throw;
            }
        }

        public void BulkUpdate<TEntity>(IList<TEntity> entities)
            where TEntity : class
        {
            try
            {
                _dbContext.Update(entities);
                _dbContext.SaveChanges();
            }
            catch
            {
                _isError = true;
                throw;
            }
        }

        public async Task<int> ExecuteSqlCommandAsync(string commandText, params object[] paras)
        {
            try
            {
                return await _dbContext.Database.ExecuteSqlCommandAsync(commandText, paras);
            }
            catch
            {
                _isError = true;
                throw;
            }
        }

        public void Dispose()
        {
            if (_isError)
            {
                _dbContext.Dispose();
            }
            else
            {
                ((IDisposable)_lease)?.Dispose();
            }
        }

        public void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch
            {
                _isError = true;
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _isError = true;
                throw;
            }
        }

        public async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Update(entity);
            await SaveChangesAsync();
        }

        public async Task AddRangeAsync<TEntity>(IList<TEntity> entities)
            where TEntity : class
        {
            await _dbContext.AddRangeAsync(entities);
            await SaveChangesAsync();
        }

        public async Task UpdateRangeAsync<TEntity>(IList<TEntity> entities)
            where TEntity : class
        {
            _dbContext.UpdateRange(entities);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Remove(entity);
            await SaveChangesAsync();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="conn">连接对象</param>
        /// <param name="tableName">将泛型集合插入到本地数据库表的表名</param>
        /// <param name="list">要插入大泛型集合</param>
        public void BulkInsert<T>(SqlConnection conn, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))

                    .Cast<PropertyDescriptor>()
                    .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                    .ToArray();

                Dictionary<int, PropertyDescriptor> orders = new Dictionary<int, PropertyDescriptor>();
                foreach (var propertyInfo in props)
                {
                    var attribute = propertyInfo.Attributes[typeof(ColumnAttribute)] as ColumnAttribute;
                    orders.Add(attribute.Order, propertyInfo);
                }

                foreach (var order in orders.OrderBy(x => x.Key))
                {
                    bulkCopy.ColumnMappings.Add(order.Value.Name, order.Value.Name);
                    table.Columns.Add(order.Value.Name, Nullable.GetUnderlyingType(order.Value.PropertyType) ?? order.Value.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        public async Task RemoveRangeAsync<TEntity>(IList<TEntity> entity)
            where TEntity : class
        {
            _dbContext.RemoveRange(entity);
            await SaveChangesAsync();
        }
    }
}
