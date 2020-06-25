using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     Ioc容器注册扩展
    /// </summary>
    public static class IocRegistrarExtensions
    {
        #region RegisterTypeIfNot

        /// <summary>
        ///     如果给定类型不存在于Ioc容器中的话以自注册的形式注册到Ioc容器.
        /// </summary>
        /// <typeparam name="T">将要注册到Ioc容器中的类型</typeparam>
        /// <param name="iocRegistrar">Ioc注册器</param>
        /// <param name="lifeStyle">生命周期</param>
        public static bool RegisterTypeIfNot<T>(this IIocRegistrar iocRegistrar,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where T : class
        {
            if (iocRegistrar.IsRegistered<T>())
            {
                return false;
            }

            iocRegistrar.RegisterType<T>(lifeStyle);
            return true;
        }

        /// <summary>
        ///     如果给定类型不存在于Ioc容器中的话以自注册的形式注册到Ioc容器.
        /// </summary>
        /// <param name="iocRegistrar">Ioc注册器</param>
        /// <param name="type">将要注册到Ioc容器中的类型</param>
        /// <param name="lifeStyle">生命周期</param>
        public static bool RegisterTypeIfNot(this IIocRegistrar iocRegistrar, Type type,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.RegisterType(type, lifeStyle);
            return true;
        }

        /// <summary>
        ///     如果给定类型不存在于Ioc容器中的话以指定实现类型的形式注册到Ioc容器
        /// </summary>
        /// <typeparam name="TType">注册类型</typeparam>
        /// <typeparam name="TImpl">实现了 <typeparamref name="TType" /> 的类型</typeparam>
        /// <param name="iocRegistrar">Ioc注册器</param>
        /// <param name="lifeStyle">生命周期</param>
        public static bool RegisterTypeIfNot<TType, TImpl>(this IIocRegistrar iocRegistrar,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            if (iocRegistrar.IsRegistered<TType>())
            {
                return false;
            }

            iocRegistrar.RegisterType<TType, TImpl>(lifeStyle);
            return true;
        }


        /// <summary>
        ///     如果给定类型不存在于Ioc容器中的话以指定实现类型的形式注册到Ioc容器
        /// </summary>
        /// <param name="iocRegistrar">Ioc注册器</param>
        /// <param name="type">注册类型</param>
        /// <param name="impl">实现了 <paramref name="type" /> 的类型</param>
        /// <param name="lifeStyle">生命周期</param>
        public static bool RegisterTypeIfNot(this IIocRegistrar iocRegistrar, Type type, Type impl,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.RegisterType(type, impl, lifeStyle);
            return true;
        }

        #endregion
    }
}
