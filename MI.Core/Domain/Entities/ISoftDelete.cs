using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Entities
{
    /// <summary>
    ///     软删除定义接口.
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        ///     标识实体是否 '已删除'.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
