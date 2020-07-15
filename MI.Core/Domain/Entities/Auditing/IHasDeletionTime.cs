using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Entities.Auditing
{
    /// <summary>
    /// 使得实体拥有 <see cref="DeletionTime"/> .
    /// <see cref="DeletionTime"/> 属性将会在<see cref="Entity"/>删除的时候自动赋值上.
    /// </summary>
    public interface IHasDeletionTime : ISoftDelete
    {
        /// <summary>
        ///     实体的删除时间.
        /// </summary>
        DateTime? DeletionTime { get; set; }
    }
}
