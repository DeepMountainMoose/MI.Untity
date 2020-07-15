using Castle.Core.Logging;
using MI.Core;
using MI.Core.Dependency;
using MI.Core.Domain.Entities;
using MI.Core.Domain.Entities.Auditing;
using MI.Core.Domain.Uow;
using MI.Core.Events.Bus;
using MI.Core.Events.Bus.Entities;
using MI.Core.Extensions;
using MI.Core.Reflection;
using MI.Core.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MI.EntityFrameworkCore.EntityFrameworkCore
{
    public class FeIDbContext : DbContext, ITransientDependency
    {
        /// <summary>
        ///     实体变更事件触发帮助类
        /// </summary>
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        ///     日志记录器.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///     事件总线.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        ///     GUID产生器.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        ///     当前的工作单元提供者.
        /// </summary>
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(FeIDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="options"></param>
        protected FeIDbContext(DbContextOptions options)
            : base(options)
        {
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = NullLogger.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
            EventBus = NullEventBus.Instance;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ConfigureGlobalFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        public override int SaveChanges()
        {
            var changeReport = ApplyConcepts();
            var result = base.SaveChanges();
            EntityChangeEventHelper.TriggerEvents(changeReport);
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var changeReport = ApplyConcepts();
            var result = await base.SaveChangesAsync(cancellationToken);
            await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
            return result;
        }

        protected virtual EntityChangeReport ApplyConcepts()
        {
            var changeReport = new EntityChangeReport();
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyConcepts(entry, changeReport);
            }
            return changeReport;
        }

        protected virtual void ApplyConcepts(EntityEntry entry, EntityChangeReport changeReport)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry, changeReport);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForModifiedEntity(entry, changeReport);
                    break;
                case EntityState.Deleted:
                    ApplyConceptsForDeletedEntity(entry, changeReport);
                    break;
            }

            AddDomainEvents(changeReport.DomainEvents, entry.Entity);

        }

        protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry, EntityChangeReport changeReport)
        {
            CheckAndSetId(entry);
            SetCreationAuditProperties(entry.Entity);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
        }

        protected virtual void ApplyConceptsForModifiedEntity(EntityEntry entry, EntityChangeReport changeReport)
        {
            SetModificationAuditProperties(entry.Entity);
            if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
            {
                SetDeletionAuditProperties(entry.Entity);
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
            }
            else
            {
                changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
            }
        }

        protected virtual void ApplyConceptsForDeletedEntity(EntityEntry entry, EntityChangeReport changeReport)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity);
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }

        protected virtual void AddDomainEvents(List<DomainEventEntry> domainEvents, object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            domainEvents.AddRange(generatesDomainEventsEntity.DomainEvents.Select(eventData => new DomainEventEntry(entityAsObj, eventData)));
            generatesDomainEventsEntity.DomainEvents.Clear();
        }


        protected virtual void SetCreationAuditProperties(object entityAsObj)
        {
            EntityAuditingHelper.SetCreationAuditProperties(entityAsObj);
        }

        protected virtual void SetModificationAuditProperties(object entityAsObj)
        {
            EntityAuditingHelper.SetModificationAuditProperties(entityAsObj);
        }


        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected virtual void SetDeletionAuditProperties(object entityAsObj)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                if (entity.DeletionTime == null)
                {
                    entity.DeletionTime = Clock.Now;
                }
            }


        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            //Set GUID Ids
            var entity = entry.Entity as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var dbGeneratedAttr = ReflectionHelper
                    .GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(
                        entry.Property("Id").Metadata.PropertyInfo
                    );

                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType == null && ShouldFilterEntity<TEntity>(entityType))
            {
                var filterExpression = CreateFilterExpression<TEntity>();
                if (filterExpression != null)
                {
                    if (entityType.IsQueryType)
                    {
                        modelBuilder.Query<TEntity>().HasQueryFilter(filterExpression);
                    }
                    else
                    {
                        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
                    }
                }
            }
        }

        protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return false;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
                expression = softDeleteFilter;
            }
            return expression;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }

    }
}
