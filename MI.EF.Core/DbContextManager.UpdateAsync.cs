using MI.Component.Core.Exceptions;
using MI.EF.Core.BulkInsert;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI.EF.Core
{
    public partial class DbContextManager<TDbContext> : IDbContextUpdateAsyncManager
        where TDbContext : DbContextBase
    {
        private readonly Action<DbUpdateNotificationEventArgs> notify;

        internal DbContextManager(string connectionString, string readOnlyConnectionString, Action<DbUpdateNotificationEventArgs> notify)
            : this(connectionString, readOnlyConnectionString)
        {
            this.notify = notify;
        }

        internal DbContextManager(string connectionString, Action<DbUpdateNotificationEventArgs> notify)
            : this(connectionString)
        {
            this.notify = notify;
        }

        public async Task<int> BulkInsertAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            ParameterChecker.CheckNull(entities, "entities");
            if (entities.Any())
            {
                return 0;
            }

            var copyEntites = entities.ToList();
            foreach (var copyEntite in copyEntites)
            {
                var dbInsertTracking = copyEntite as IDbInsertTracking;
                if (dbInsertTracking != null)
                {
                    if (dbInsertTracking.CreateTime == DateTime.MinValue)
                    {
                        dbInsertTracking.CreateTime = DateTime.Now;
                    }
                    if (dbInsertTracking.LastUpdateTime == DateTime.MinValue)
                    {
                        dbInsertTracking.LastUpdateTime = DateTime.Now;
                    }
                }
            }
            var result = await this.InternalActionAsync(db =>
            {
                var bulkInsert = ProviderFactory.Get(db);
                bulkInsert.Run<TEntity>(entities);
            });

            return copyEntites.Count;
        }

        public async Task<int> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            ParameterChecker.CheckNull(entity, "entity");
            var dbInsertTracking = entity as IDbInsertTracking;
            if (dbInsertTracking != null)
            {
                if (dbInsertTracking.CreateTime == DateTime.MinValue)
                {
                    dbInsertTracking.CreateTime = DateTime.Now;
                }

                if (dbInsertTracking.LastUpdateTime == DateTime.MinValue)
                {
                    dbInsertTracking.LastUpdateTime = DateTime.Now;
                }
            }

            var result = await this.InternalActionAsync(db => db.Set<TEntity>().Add(entity));
            var notification = new DbUpdateNotificationEventArgs<TEntity>(DbUpdateNotification.Insert);
            notification.UpdatedEntities.Add(entity);
            notify?.Invoke(notification);

            return result;
        }

        public async Task<bool> UpdateAsync<TEntity>(TEntity entity, Action<TEntity> update) where TEntity : class
        {
            ParameterChecker.CheckNull(entity, "entity");
            ParameterChecker.CheckNull(update, "update");

            var row = await this.InternalActionAsync(db =>
            {
                db.Set<TEntity>().Attach(entity);

                var now = DateTime.Now;
                var dbUpdateTracking = entity as IDbUpdateTracking;
                if (dbUpdateTracking != null)
                {
                    now = dbUpdateTracking.LastUpdateTime;
                }

                update(entity);
                if (dbUpdateTracking != null && dbUpdateTracking.LastUpdateTime == now)
                {
                    dbUpdateTracking.LastUpdateTime = DateTime.Now;
                }
            });

            var notification = new DbUpdateNotificationEventArgs<TEntity>(DbUpdateNotification.Update);
            notification.UpdatedEntities.Add(entity);
            notify?.Invoke(notification);

            return row > 0;
        }

        public async Task<bool> UpdateAsync<TEntity>(int pkid, params IDifferUpdateProperty<TEntity>[] updateProperty) where TEntity : class
        {
            return await UpdateAsync(new long[] { pkid }, updateProperty);
        }

        public async Task<bool> UpdateAsync<TEntity>(IEnumerable<int> pkids, params IDifferUpdateProperty<TEntity>[] updateProperties) where TEntity : class
        {
            if (pkids == null)
            {
                return false;
            }
            var pkid_int = pkids.ToArray();
            var pkid_long = new long[pkids.Count()];
            for (int i = 0; i < pkid_long.Length; i++)
            {
                pkid_long[i] = pkid_int[i];
            }

            return await UpdateAsync(pkid_long, updateProperties);
        }

        public async Task<bool> UpdateAsync<TEntity>(long pkid, params IDifferUpdateProperty<TEntity>[] updateProperties) where TEntity : class
        {
            return await UpdateAsync(new long[] { pkid }, updateProperties);
        }

        public async Task<bool> UpdateAsync<TEntity>(IEnumerable<long> pkids, params IDifferUpdateProperty<TEntity>[] updateProperties) where TEntity : class
        {
            ParameterChecker.CheckNull(pkids, "pkids");
            ParameterChecker.CheckNull(updateProperties, "updateProperties");

            if (!pkids.Any())
            {
                return false;
            }

            if (!updateProperties.Any())
            {
                return false;
            }

            pkids = pkids.Distinct();
            foreach (var propertyGroup in updateProperties.GroupBy(_ => _.Property))
            {
                if (propertyGroup.Count() > 1)
                {
                    throw new DbContextException(0, $"{typeof(TEntity).Name}更新包含{propertyGroup.Count()}个更新属性{propertyGroup.Key}");
                }
            }

            var clonedProperties = updateProperties.ToList();

            var propLastUpdateTime = typeof(TEntity).GetProperty("LastUpdateTime");
            if (propLastUpdateTime != null)
            {
                string columnNameLastUpdateTime = "LastUpdateTime";
                var ColumnAttributeData = propLastUpdateTime.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Equals(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)));
                if (ColumnAttributeData != null && ColumnAttributeData.ConstructorArguments.Count > 0)
                {
                    columnNameLastUpdateTime = ColumnAttributeData.ConstructorArguments[0].Value.ToString();
                }
                if (typeof(IDbUpdateTracking).IsAssignableFrom(typeof(TEntity)) &&
                !updateProperties.Any(_ => string.Equals(_.Property, "LastUpdateTime", StringComparison.InvariantCultureIgnoreCase)) &&
                !updateProperties.Any(_ => string.Equals(_.Property, columnNameLastUpdateTime, StringComparison.InvariantCultureIgnoreCase)))
                {
                    clonedProperties.Add(new UpdateProperty<TEntity, DateTime>(columnNameLastUpdateTime, DateTime.Now, false, string.Empty));
                }
            }

            var stringBuilder = new StringBuilder();
            var parameters = new List<SqlParameter>();
            var andWheres = new List<string>();
            foreach (var differProperty in clonedProperties)
            {
                var sqlParameter = $"@{differProperty.Property}";
                if(differProperty.IsDiffer)
                {
                    stringBuilder.Append($"{differProperty.Property} = {differProperty.Property} + {sqlParameter},");

                    if (!string.IsNullOrWhiteSpace(differProperty.UpdateConstraint))
                    {
                        andWheres.Add($"{differProperty.Property} + {sqlParameter} {differProperty.UpdateConstraint}");
                    }
                }
                else
                {
                    stringBuilder.Append($"{differProperty.Property} = {sqlParameter},");
                }

                parameters.Add(new SqlParameter(sqlParameter, differProperty.Value));
            }

            List<string> lstCommand = new List<string>();
            SqlParameter pkidParameter = null;
            int batchCount = 50;
            string tableName = typeof(TEntity).Name;
            var TableAttributeData = typeof(TEntity).CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Equals(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)));
            if (TableAttributeData != null && TableAttributeData.ConstructorArguments.Count > 0)
            {
                tableName = TableAttributeData.ConstructorArguments[0].Value.ToString();
            }
            for(int i=0; ;i++)
            {
                long[] CurrentIds = pkids.Skip(i * batchCount).Take(batchCount).ToArray();
                if(CurrentIds.Length==0)
                {
                    break;
                }

                var updateCommandText = $"UPDATE {tableName} " +
                                        $"SET {stringBuilder.ToString().Substring(0, stringBuilder.Length - 1)} ";

                if (pkids.Count() == 1)
                {
                    updateCommandText += $"WHERE Id = @Id ";
                    pkidParameter = new SqlParameter("@Id", pkids.First());
                }
                else
                {
                    updateCommandText += $"WHERE PKID IN ({string.Join(",", CurrentIds)}) ";
                }

                foreach (var andWhere in andWheres)
                {
                    updateCommandText += $"AND {andWhere} ";
                }

                lstCommand.Add(updateCommandText);
            }

            if (pkidParameter != null)
            {
                parameters.Add(pkidParameter);
            }

            var executedCommand = 0;
            await this.InternalActionAsync(async db =>
            {
                if (lstCommand.Count > 1)
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        for (int i = 0; i < lstCommand.Count; i++)
                        {
                            var lstParameter = new List<DbParameter>();
                            using (var cmd = db.Database.GetDbConnection().CreateCommand())
                            {
                                foreach (var sqlParameter in parameters)
                                {
                                    var para = cmd.CreateParameter();
                                    para.ParameterName = sqlParameter.ParameterName;
                                    para.Value = sqlParameter.Value;
                                    lstParameter.Add(para);
                                }
                            }
                            executedCommand += await db.Database.ExecuteSqlRawAsync(lstCommand[i], lstParameter.ToArray());
                        }
                        await db.SaveChangesAsync();
                        tran.Commit();
                    }
                }
                else if (lstCommand.Count == 1)
                {
                    var lstParameter = new List<DbParameter>();
                    using (var cmd = db.Database.GetDbConnection().CreateCommand())
                    {
                        foreach (var sqlParameter in parameters)
                        {
                            var para = cmd.CreateParameter();
                            para.ParameterName = sqlParameter.ParameterName;
                            para.Value = sqlParameter.Value;
                            lstParameter.Add(para);
                        }
                    }
                    executedCommand = await db.Database.ExecuteSqlRawAsync(lstCommand[0], lstParameter.ToArray());
                }
            });

            var notification = new DbUpdateNotificationEventArgs<TEntity>(DbUpdateNotification.Update);
            notification.Ids.AddRange(pkids);
            notify?.Invoke(notification);

            return executedCommand > 0;
        }

        private async Task<int> InternalActionAsync(Action<TDbContext> action)
        {
            ParameterChecker.CheckNull(action, "action");
            var db = (TDbContext)Activator.CreateInstance(typeof(TDbContext), this.connectionString);
            using (db)
            {
                return await this.dbScope.ExecuteAsync(async () =>
                {
                    action(db);

                    return await db.SaveChangesAsync();
                });
            }
        }

        private async Task<int> InternalActionAsync(Func<TDbContext, Task> action)
        {
            ParameterChecker.CheckNull(action, "action");
            var db = (TDbContext)Activator.CreateInstance(typeof(TDbContext), this.connectionString);
            using (db)
            {
                return await this.dbScope.ExecuteAsync(async () =>
                {
                    await action(db);

                    return await db.SaveChangesAsync();
                });
            }
        }
    }
}
