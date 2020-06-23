using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Configuration
{
    /// <summary>
    ///     定义一个存在配置用的Dictionary.
    /// </summary>
    public interface IDictionaryBasedConfig
    {
        /// <summary>
        ///     通过设置name来配置.
        ///     如果存在相同的 <paramref name="name" />, 则覆盖配置.
        /// </summary>
        /// <param name="name">
        ///     配置的唯一标识名称
        /// </param>
        /// <param name="value">
        ///     配置的值
        /// </param>
        void Set<T>(string name, T value);

        /// <summary>
        ///     通过<paramref name="name" />获取配置对象.
        /// </summary>
        /// <param name="name">配置的唯一标识名称</param>
        /// <returns>该配置的值或者null</returns>
        object Get(string name);

        /// <summary>
        ///     通过<paramref name="name" />获取配置对象.
        /// </summary>
        /// <param name="name">
        ///     配置的唯一标识名称
        /// </param>
        /// <typeparam name="T">
        ///     配置的对象类型
        /// </typeparam>
        /// <returns>
        ///     该配置的值或者null
        /// </returns>
        T Get<T>(string name);

        /// <summary>
        ///     通过<paramref name="name" />获取配置对象.
        /// </summary>
        /// <param name="name">
        ///     配置的对象类型
        /// </param>
        /// <param name="defaultValue">
        ///     如果未根据给定的name找到配置,则返回指定的默认值
        /// </param>
        /// <returns>
        ///     该配置的值或者<paramref name="defaultValue" />
        /// </returns>
        object Get(string name, object defaultValue);

        /// <summary>
        ///     通过<paramref name="name" />获取配置对象.
        /// </summary>
        /// <typeparam name="T">
        ///     配置的对象类型
        /// </typeparam>
        /// <param name="name">
        ///     配置的唯一标识名称
        /// </param>
        /// <param name="defaultValue">
        ///     如果未找到指定配置,则返回指定的默认值
        /// </param>
        /// <returns>
        ///     该配置的值或者<paramref name="defaultValue" />
        /// </returns>
        T Get<T>(string name, T defaultValue);

        /// <summary>
        ///     通过<paramref name="name" />获取配置对象.
        /// </summary>
        /// <typeparam name="T">
        ///     配置的对象类型
        /// </typeparam>
        /// <param name="name">
        ///     配置的唯一标识名称
        /// </param>
        /// <param name="creator">
        ///     如果未找到指定配置,则创建该配置
        /// </param>
        /// <returns>
        ///     该配置的值
        /// </returns>
        T GetOrCreate<T>(string name, Func<T> creator);
    }
}
