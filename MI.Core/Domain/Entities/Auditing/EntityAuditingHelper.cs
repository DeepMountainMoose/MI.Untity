using MI.Core.Extensions;
using MI.Core.Timing;
using System;

namespace MI.Core.Domain.Entities.Auditing
{
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(
            object entityAsObj)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default(DateTime))
            {
                entityWithCreationTime.CreationTime = Clock.Now;
            }
        }

        public static void SetModificationAuditProperties(
            object entityAsObj)
        {
            if (entityAsObj is IHasModificationTime)
            {
                entityAsObj.As<IHasModificationTime>().LastModificationTime = Clock.Now;
            }
        }
    }
}
