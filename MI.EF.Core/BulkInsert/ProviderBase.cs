using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MI.EF.Core.BulkInsert
{
    public abstract class ProviderBase<TConnection, TTransaction> : IEfBulkInsertProvider where TConnection : IDbConnection where TTransaction : IDbTransaction
    {
        public DbContext Context
        {
            get;
            private set;
        }

        protected virtual string ConnectionString
        {
            get { return (string)this.DbConnection.ConnectionString; }
        }

        protected virtual IDbConnection DbConnection
        {
            get { return this.Context.Database.GetDbConnection(); }
        }

        public IEfBulkInsertProvider SetContext(DbContext context)
        {
            this.Context = context;
            return this;
        }

        public IDbConnection GetConnection()
        {
            return this.CreateConnection();
        }

        protected abstract TConnection CreateConnection();

        public void Run<T>(IEnumerable<T> entities, IDbTransaction transaction)
        {
            this.Run<T>(entities, (TTransaction)((object)transaction));
        }

        public virtual void Run<T>(IEnumerable<T> entities)
        {
            using (IDbConnection dbConnection = this.GetConnection())
            {
                dbConnection.Open();
                using (IDbTransaction transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        this.Run<T>(entities, transaction);
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        if(transaction.Connection!=null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }

        public abstract void Run<T>(IEnumerable<T> entities, TTransaction transaction);
    }
}
