using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Entities.Auditing
{
    /// <summary>
    ///     An entity can implement this interface if <see cref="LastModificationTime" /> of this entity must be stored.
    ///     <see cref="LastModificationTime" /> is automatically set when updating <see cref="Entity" />.
    ///     <para>
    ///         当一个<see cref="IEntity" />继承自此接口的时候, 其<see cref="LastModificationTime" />将会在实体更新的时候自动设置
    ///     </para>
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        ///     The last modified time for this entity.
        ///     <para>
        ///         当前实体最后修改时间.
        ///     </para>
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }
}
