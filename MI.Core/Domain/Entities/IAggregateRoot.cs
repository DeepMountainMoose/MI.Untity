using System;
using System.Collections.Generic;
using MI.Core.Events.Bus;

namespace MI.Core.Domain.Entities
{
    /// <summary>
    ///     聚合根对象
    /// </summary>
    public interface IAggregateRoot : IAggregateRoot<int>, IEntity
    {

    }

    /// <summary>
    ///     聚合根对象,继承自<see cref="IEntity{TPrimaryKey}"/>和<see cref="IGeneratesDomainEvents"/>
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>, IGeneratesDomainEvents
    {

    }

    /// <summary>
    ///     包含领域事件的对象
    /// </summary>
    public interface IGeneratesDomainEvents
    {
        /// <summary>
        ///     领域事件集合
        /// </summary>
        ICollection<IEventData> DomainEvents { get; }
    }
}
