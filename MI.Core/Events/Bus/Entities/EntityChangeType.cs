using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Events.Bus.Entities
{
    /// <summary>
    ///     实体变更类型
    /// </summary>
    public enum EntityChangeType
    {
        /// <summary>
        ///     创建
        /// </summary>
        Created,
        /// <summary>
        ///     修改
        /// </summary>
        Updated,
        /// <summary>
        ///     删除
        /// </summary>
        Deleted
    }
}
