using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class DbUpdateNotificationEventArgs<TEntity> : DbUpdateNotificationEventArgs
        where TEntity : class
    {
        public DbUpdateNotificationEventArgs(DbUpdateNotification notification)
            : base(typeof(TEntity), notification)
        {

        }
    }
}
