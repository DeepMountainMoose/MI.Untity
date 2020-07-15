using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Entities.Auditing
{
    /// <summary>
    ///     An entity can implement this interface if <see cref="CreationTime" /> of this entity must be stored.
    ///     <see cref="CreationTime" /> is automatically set when saving <see cref="Entity" /> to database.
    ///     <para>
    ///         当一个<see cref="IEntity" />继承自此接口的时候, 其<see cref="CreationTime" />将会在实体创建的时候自动设置
    ///     </para>
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        ///     Creation time of this entity.
        ///     <para>
        ///         当前实体的创建时间.
        ///     </para>
        /// </summary>
        DateTime CreationTime { get; set; }
    }
}
