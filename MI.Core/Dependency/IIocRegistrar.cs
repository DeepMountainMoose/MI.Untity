using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     定义用于注册依赖项的接口
    /// </summary>
    public interface IIocRegistrar
    {
        /// <summary>
        ///     添加注册惯例
        /// </summary>
        /// <param name="registrar">
        ///     惯例配置注册对象
        /// </param>
        void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar);

        /// <summary>
        ///     根据当前已有惯例来注册程序集里的所有类型
        /// </summary>
        /// <param name="assembly">
        ///     将要注册的程序集
        /// </param>
        void RegisterAssemblyByConvention(Assembly assembly);

        /// <summary>
        ///     根据当前已有惯例来注册程序集里的所有类型
        /// </summary>
        /// <param name="assembly">
        ///     Assembly to register
        ///     <para>被注册的程序集</para>
        /// </param>
        /// <param name="config">
        ///     额外的配置
        /// </param>
        void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config);

        /// <summary>
        ///     通过自注册的形式注册一个类型
        /// </summary>
        /// <typeparam name="T">
        ///     注册的类型
        /// </typeparam>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        void RegisterType<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where T : class;

        /// <summary>
        ///     通过自注册的形式注册一个类型
        /// </summary>
        /// <param name="type">
        ///     注册的类型
        /// </param>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        void RegisterType(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     通过实现类型的形式注册一个类型
        /// </summary>
        /// <typeparam name="TType">
        ///     注册的类型
        /// </typeparam>
        /// <typeparam name="TImpl">
        ///     注册类型的实现类
        /// </typeparam>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        void RegisterType<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType;

        /// <summary>
        ///     通过实现类型的形式注册一个类型
        /// </summary>
        /// <param name="type">
        ///     注册的类型
        /// </param>
        /// <param name="impl">
        ///     注册类型的实现类
        /// </param>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        void RegisterType(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        ///     通过实例的形式注册一个类型
        /// </summary>
        /// <typeparam name="TType">
        ///     注册的类型
        /// </typeparam>
        /// <param name="instance">
        ///     注册的实例
        /// </param>
        void RegisterInstance<TType>(TType instance)
            where TType : class;

        /// <summary>
        ///     通过实例的形式注册一个类型
        /// </summary>
        /// <param name="type">
        ///     注册的类型
        /// </param>
        /// <param name="instance">
        ///     注册的实例
        /// </param>
        void RegisterInstance(Type type, object instance);

        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <param name="type">
        ///     需要检查是否已经注册的类型
        /// </param>
        bool IsRegistered(Type type);

        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <typeparam name="T">
        ///     需要检查是否已经注册的类型
        /// </typeparam>
        bool IsRegistered<T>();
    }
}
