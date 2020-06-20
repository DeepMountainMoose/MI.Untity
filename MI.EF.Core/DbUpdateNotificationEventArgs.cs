using System;
using System.Collections.Generic;
using System.Text;

namespace MI.EF.Core
{
    public class DbUpdateNotificationEventArgs : EventArgs
    {
        public DbUpdateNotificationEventArgs(Type entityType, DbUpdateNotification notification)
        {
            this.EntityType = entityType;
            this.Notification = notification;
            this.Ids = new List<long>();
            this.UpdatedEntities = new List<object>();
        }

        public Type EntityType { get; private set; }
        public DbUpdateNotification Notification { get; private set; }
        public List<long> Ids { get; private set; }
        public List<object> UpdatedEntities { get; private set; }
    }

    public enum DbUpdateNotification
    {
        Insert,
        Update
    }
}
