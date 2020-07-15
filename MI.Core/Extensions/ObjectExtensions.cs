using System;
using System.Globalization;
using System.Linq;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     定义对于所有类型的扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     简化的as类型转换
        /// </summary>
        /// <typeparam name="T">
        ///     将会被转换到的类型
        /// </typeparam>
        /// <param name="obj">
        ///     将会被转换的对象
        /// </param>
        /// <returns>
        ///     转换后的类型,或null(转换失败)
        /// </returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        /// <summary>
        ///     使用 <see cref="Convert.ChangeType(object,TypeCode)" /> 方法来转换指定类型
        /// </summary>
        /// <param name="obj">
        ///     将要被转换的类型
        /// </param>
        /// <typeparam name="T">
        ///     要转换到的目标类型
        /// </typeparam>
        /// <returns>
        ///     转换后的实例
        /// </returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     检查给定Item是否存在于List中
        /// </summary>
        /// <param name="item">
        ///     将要被检查的Item
        /// </param>
        /// <param name="list">
        ///     要被检查的可能包含Item的List
        /// </param>
        /// <typeparam name="T">
        ///     Item的类型
        /// </typeparam>
        /// <returns>
        ///     该Item如果存在于List则为true,否则为false
        /// </returns>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        ///     检查给定Item是否存在于List中
        /// </summary>
        /// <param name="item">
        ///     将要被检查的Item
        /// </param>
        /// <param name="args1">
        ///     包含的Item
        /// </param>
        /// <typeparam name="T">
        ///     Item的类型
        /// </typeparam>
        /// <returns>
        ///     该Item如果存在于List则为true,否则为false
        /// </returns>
        public static bool IsIn<T>(this T item, T args1)
        {
            return args1.Equals(item);
        }

        /// <summary>
        ///     检查给定Item是否存在于List中
        /// </summary>
        /// <param name="item">
        ///     将要被检查的Item
        /// </param>
        /// <param name="args1">
        ///     包含的Item
        /// </param>
        /// <param name="args2">
        ///     包含的Item
        /// </param>
        /// <typeparam name="T">
        ///     Item的类型
        /// </typeparam>
        /// <returns>
        ///     该Item如果存在于List则为true,否则为false
        /// </returns>
        public static bool IsIn<T>(this T item, T args1, T args2)
        {
            if (item == null)
                return false;
            return args1.Equals(item) || args2.Equals(item);
        }

        /// <summary>
        ///     检查给定Item是否存在于List中
        /// </summary>
        /// <param name="item">
        ///     将要被检查的Item
        /// </param>
        /// <param name="args1">
        ///     包含的Item
        /// </param>
        /// <param name="args2">
        ///     包含的Item
        /// </param>
        /// <param name="args3">
        ///     包含的Item
        /// </param>
        /// <typeparam name="T">
        ///     Item的类型
        /// </typeparam>
        /// <returns>
        ///     该Item如果存在于List则为true,否则为false
        /// </returns>
        public static bool IsIn<T>(this T item, T args1, T args2, T args3)
        {
            return args1.Equals(item) || args2.Equals(item) || args3.Equals(item);
        }
    }
}
