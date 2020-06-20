using MI.Component.Core.Env;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core.Env
{
    public abstract class EnvironmentHandlerBase<TDbContext> :
        IEnvironmentHandler<TDbContext>,
        IInitializeEnvironment
        where TDbContext : DbContextBase
    {
        public IDbContextManager<TDbContext> Db => !string.IsNullOrEmpty(this.DBReadOnlyConnectionString) ?
            new DbContextManager<TDbContext>(this.DBConnectionString, this.DBReadOnlyConnectionString, NotifyDbUpdate) :
            new DbContextManager<TDbContext>(this.DBConnectionString, NotifyDbUpdate);
        protected virtual string DBConnectionString
        {
            get;
        }

        protected virtual string DBReadOnlyConnectionString
        {
            get;
        }

        public virtual void Initialize()
        {
            DbContextPreloadManager.Preload<TDbContext>(this.DBConnectionString);
        }

        public virtual void NotifyDbUpdate(DbUpdateNotificationEventArgs e)
        {

        }
    }
}
