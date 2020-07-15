using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Domain.Entities
{
    /// <summary>
    ///     以<see cref="int" />作为主键的实体接口
    /// </summary>
    public interface IEntity : IEntity<int>
    {
    }

    /// <summary>
    ///     实体的基础接口,所有实体都必须实现此接口
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity<para>主键类型</para></typeparam>
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        ///     实体的唯一标识(主键)
        /// </summary>
        TPrimaryKey Id { get; set; }

        /// <summary>
        ///     该实体是否为临时的(如果没有持久化到数据库的话将不会有Id)
        /// </summary>
        /// <returns></returns>
        bool IsTransient();
    }
}
