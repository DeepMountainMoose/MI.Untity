using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     定义用于解析依赖项的接口
    /// </summary>
    public interface IIocResolver
    {
        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <typeparam name="T">
        ///     需要返回的指定类型
        /// </typeparam>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        T Resolve<T>();

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <typeparam name="T">
        ///     需要返回的指定类型
        /// </typeparam>
        /// <param name="argumentsAsAnonymousType">
        ///     构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        T Resolve<T>(object argumentsAsAnonymousType);

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <param name="type">
        ///     需要返回的指定类型
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        object Resolve(Type type);

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <param name="type">
        ///     需要返回的指定类型
        /// </param>
        /// <param name="argumentsAsAnonymousType">
        ///     构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        object Resolve(Type type, object argumentsAsAnonymousType);

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <typeparam name="T">
        ///     将要获取的对象类型
        /// </typeparam>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        T[] ResolveAll<T>();

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <typeparam name="T">
        ///     将要获取的对象类型
        /// </typeparam>
        /// <param name="argumentsAsAnonymousType">
        ///     构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        T[] ResolveAll<T>(object argumentsAsAnonymousType);

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <param name="type">
        ///     将要获取的对象类型
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        object[] ResolveAll(Type type);

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <param name="type">
        ///     将要获取的对象类型
        /// </param>
        /// <param name="argumentsAsAnonymousType">
        ///     构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        object[] ResolveAll(Type type, object argumentsAsAnonymousType);

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

        /// <summary>
        ///     释放一个对象
        /// </summary>
        /// <param name="obj">
        ///     将要被释放的对象
        /// </param>
        void Release(object obj);
    }
}
