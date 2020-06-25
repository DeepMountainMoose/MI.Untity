using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     <see cref="ICollection{T}" />的扩展方法.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Checks whatever given collection object is null or has no item.
        ///     <para>检查给定集合是否空或者无数据</para>
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        ///     将数据添加到给定集合.如果给定数据已存在则不添加
        /// </summary>
        /// <param name="source">
        ///     给定集合
        /// </param>
        /// <param name="item">
        ///     给定数据,将会检查是否存在后添加
        /// </param>
        /// <typeparam name="T">
        ///     集合所存储的数据类型
        /// </typeparam>
        /// <returns>
        ///     返回true则表示成功将item添加到集合,返回false则表示未添加(可能该item已存在)
        /// </returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Contains(item))
                return false;

            source.Add(item);
            return true;
        }
    }
}
