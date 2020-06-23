using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MI.Core.Dependency
{
    /// <summary>
    ///     <see cref="IIocManager" /> 的默认实现.
    ///     用于管理所有依赖注入的实现类.
    /// </summary>
    [DebuggerDisplay(
         "IocManager, Implement by {IocContainer.GetType().Name} Component {IocContainer.Kernel.GraphNodes.Length}, Conventional {_conventionalRegistrars.Count}"
     )]
    public sealed class IocManager : IIocManager
    {
        /// <summary>
        ///     所有的注册惯例列表.
        /// </summary>
        private readonly List<IConventionalDependencyRegistrar> _conventionalRegistrars;

        /// <summary>
        ///     表示当前 <see cref="IocManager" /> 是否被释放.
        /// </summary>
        private bool _isDisposed;

        ///// <summary>
        /////     默认的依赖注入管理器实例
        ///// </summary>
        //[Obsolete("已过期，应该使用通常依赖注入的方式使用")]
        //public static IocManager Instance { get; private set; }

        /// <summary>
        ///     单例的Castle ProxyGenerator.
        ///     此实例用于避免Castle可能导致内存泄漏和性能相关问题,具体可参考:
        ///  <a href="https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md">Castle.Core documentation</a>
        /// </summary>
        private static readonly ProxyGenerator ProxyGeneratorInstance = new ProxyGenerator();

        /// <summary>
        ///     Ioc容器的引用.
        /// </summary>
        public IWindsorContainer IocContainer { get; }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                IocContainer.Dispose();
            }
        }

        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration,
            DependencyLifeStyle lifeStyle)
            where T : class
        {
            switch (lifeStyle)
            {
                case DependencyLifeStyle.Transient:
                    return registration.LifestyleTransient();
                case DependencyLifeStyle.Singleton:
                    return registration.LifestyleSingleton();
                default:
                    return registration;
            }
        }

        #region [ Ctor ]

        //static IocManager()
        //{
        //    Instance = new IocManager();
        //}

        /// <summary>
        ///     初始化一个 <see cref="IocManager" />
        /// </summary>
        public IocManager()
        {
            IocContainer = new WindsorContainer(new DefaultProxyFactory(ProxyGeneratorInstance));

            _conventionalRegistrars = new List<IConventionalDependencyRegistrar>();

            IocContainer.Register(
                Component.For<IocManager, IIocManager, IIocRegistrar, IIocResolver>().Instance(this)
            );
        }

        #endregion

        #region [ RegisterConvention ]

        /// <summary>
        ///     添加注册惯例
        /// </summary>
        /// <param name="registrar">
        ///     惯例配置注册对象
        /// </param>
        public void AddConventionalRegistrar(IConventionalDependencyRegistrar registrar)
        {
            _conventionalRegistrars.Add(registrar);
        }

        /// <summary>
        ///     根据当前已有惯例来注册程序集里的所有类型
        /// </summary>
        /// <param name="assembly">
        ///     将要注册的程序集
        /// </param>
        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            RegisterAssemblyByConvention(assembly, new ConventionalRegistrationConfig());
        }

        /// <summary>
        ///     根据当前已有惯例来注册程序集里的所有类型
        /// </summary>
        /// <param name="assembly">
        ///     被注册的程序集
        /// </param>
        /// <param name="config">
        ///     额外的配置
        /// </param>
        public void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config)
        {
            var context = new ConventionalRegistrationContext(assembly, this, config);

            foreach (var registerer in _conventionalRegistrars)
                registerer.RegisterAssembly(context);

            if (config.InstallInstallers)
                IocContainer.Install(FromAssembly.Instance(assembly));
        }

        #endregion

        #region [ Register ]

        /// <summary>
        ///     通过自注册的形式注册一个类型
        /// </summary>
        /// <typeparam name="T">
        ///     注册的类型
        /// </typeparam>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        public void RegisterType<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class
        {
            RegisterType(typeof(T), lifeStyle);
        }

        /// <summary>
        ///     通过自注册的形式注册一个类型
        /// </summary>
        /// <param name="type">
        ///     注册的类型
        /// </param>
        /// <param name="lifeStyle">
        ///     注册类型的生命周期
        /// </param>
        public void RegisterType(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            RegisterType(type, type, lifeStyle);
        }

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
        public void RegisterType<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class where TImpl : class, TType
        {
            RegisterType(typeof(TType), typeof(TImpl), lifeStyle);
        }

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
        public void RegisterType(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            IocContainer.Register(ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle));
        }

        /// <summary>
        ///     通过实例的形式注册一个类型
        /// </summary>
        /// <typeparam name="TType">
        ///     注册的类型
        /// </typeparam>
        /// <param name="instance">
        ///     注册的实例
        /// </param>
        public void RegisterInstance<TType>(TType instance) where TType : class
        {
            RegisterInstance(typeof(TType), instance);
        }

        /// <summary>
        ///     通过实例的形式注册一个类型
        /// </summary>
        /// <param name="type">
        ///     注册的类型
        /// </param>
        /// <param name="instance">
        ///     注册的实例
        /// </param>
        public void RegisterInstance(Type type, object instance)
        {
            IocContainer.Register(Component.For(type).Instance(instance));
        }

        #endregion

        #region [ Resolve ]

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <typeparam name="T">
        ///     需要返回的指定类型
        /// </typeparam>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <typeparam name="T">
        ///     需要返回的指定类型
        /// </typeparam>
        /// <param name="argumentsAsAnonymousType">
        ///     返回目标类型所需要的自定义构造函数的参数
        /// </param>
        /// <returns>
        ///     The object instance
        ///     <para>返回的对象实例</para>
        /// </returns>
        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve<T>(Arguments.FromProperties(argumentsAsAnonymousType));
        }

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <param name="type">
        ///     需要返回的指定类型
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        public object Resolve(Type type)
        {
            return IocContainer.Resolve(type);
        }

        /// <summary>
        ///     从Ioc容器中返回实例
        /// </summary>
        /// <param name="type">
        ///     需要返回的指定类型
        /// </param>
        /// <param name="argumentsAsAnonymousType">
        ///     返回目标类型所需要的自定义构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve(type, Arguments.FromProperties(argumentsAsAnonymousType));
        }

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
        public T[] ResolveAll<T>()
        {
            return IocContainer.ResolveAll<T>();
        }

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <typeparam name="T">
        ///     将要获取的对象类型
        /// </typeparam>
        /// <param name="argumentsAsAnonymousType">返回目标类型所需要的自定义构造函数的参数
        /// </param>
        /// <returns>
        ///     返回的对象实例
        /// </returns>
        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll<T>(Arguments.FromProperties(argumentsAsAnonymousType));
        }

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <param name="type">将要获取的对象类型
        /// </param>
        /// <returns>返回的对象实例
        /// </returns>
        public object[] ResolveAll(Type type)
        {
            return IocContainer.ResolveAll(type).Cast<object>().ToArray();
        }

        /// <summary>
        ///     获取所有继承自指定类型的实现类.
        ///     返回的对象必须要在使用完后被Released(see <see cref="Release" />)
        /// </summary>
        /// <param name="type">将要获取的对象类型
        /// </param>
        /// <param name="argumentsAsAnonymousType">返回目标类型所需要的自定义构造函数的参数
        /// </param>
        /// <returns>返回的对象实例
        /// </returns>
        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll(type, Arguments.FromProperties(argumentsAsAnonymousType)).Cast<object>().ToArray();
        }

        #endregion

        #region [ IsRegister ]

        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <param name="type">
        ///     需要检查是否已经注册的类型
        /// </param>
        public bool IsRegistered(Type type)
        {
            return IocContainer.Kernel.HasComponent(type);
        }

        /// <summary>
        ///     检查给定类型是否已经注册
        /// </summary>
        /// <typeparam name="T">
        ///     需要检查是否已经注册的类型
        /// </typeparam>
        public bool IsRegistered<T>()
        {
            return IocContainer.Kernel.HasComponent(typeof(T));
        }

        /// <summary>
        ///     释放一个对象
        /// </summary>
        /// <param name="obj"></param>
        public void Release(object obj)
        {
            IocContainer.Release(obj);
        }

        #endregion
    }
}
