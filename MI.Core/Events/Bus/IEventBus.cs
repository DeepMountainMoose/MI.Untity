using MI.Core.Events.Bus.Factories;
using MI.Core.Events.Bus.Handlers;
using System;
using System.Threading.Tasks;

namespace MI.Core.Events.Bus
{
    /// <summary>
    ///     定义事件总线的接口
    /// </summary>
    public interface IEventBus
    {
        #region Register

        /// <summary>
        ///     注册一个事件.可在此处注册一个委托,在事件触发时将会被执行
        /// </summary>
        /// <param name="action">
        ///     事件触发时的行为
        /// </param>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        /// <summary>
        ///     注册一个事件.可在此处注册一个委托,在事件触发时将会被执行
        /// </summary>
        /// <param name="action">
        ///     事件触发时的行为
        /// </param>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData;

        /// <summary>
        ///     注册一个事件.可在此处注册一个委托,在事件触发时将会被执行
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <param name="handler">
        ///     事件处理器对象
        /// </param>
        IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        ///     注册一个事件.
        ///     当事件触发时一个新的实例 <typeparamref name="THandler" /> 将被创建并执行.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <typeparam name="THandler">
        ///     事件处理器类型
        /// </typeparam>
        IDisposable Register<TEventData, THandler>() where TEventData : IEventData
            where THandler : IEventHandler, new();

        /// <summary>
        ///     注册一个事件.当事件触发时给定的实例将会执行.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="handler">
        ///     事件处理器对象
        /// </param>
        IDisposable Register(Type eventType, IEventHandler handler);

        /// <summary>
        ///     注册一个事件.给定的事件处理器工厂将会创建和释放时间处理器.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <param name="handlerFactory">
        ///     一个创建和释放事件处理器实例的工厂
        /// </param>
        IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData;

        /// <summary>
        ///     注册一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="handlerFactory">
        ///     一个创建和释放事件处理器实例的工厂
        /// </param>
        IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory);

        #endregion

        #region Unregister

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        /// <param name="action"></param>
        void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        /// <param name="action"></param>
        void AsyncUnregister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData;

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        /// <param name="handler">已注册的事件处理器对象</param>
        void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData;

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     Event type
        ///     <para>事件类型</para>
        /// </param>
        /// <param name="handler">已注册的事件处理器对象</param>
        void Unregister(Type eventType, IEventHandler handler);

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        /// <param name="factory">已注册的事件处理器工厂对象</param>
        void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData;

        /// <summary>
        ///     取消注册一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     Event type
        ///     <para>事件类型</para>
        /// </param>
        /// <param name="factory">已注册的事件处理器工厂对象</param>
        void Unregister(Type eventType, IEventHandlerFactory factory);

        /// <summary>
        ///     取消注册所有事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        void UnregisterAll<TEventData>() where TEventData : IEventData;

        /// <summary>
        ///     取消注册所有事件.
        /// </summary>
        /// <param name="eventType">
        ///     Event type
        ///     <para>事件类型</para>
        /// </param>
        void UnregisterAll(Type eventType);

        #endregion

        #region Trigger

        /// <summary>
        ///     触发一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <param name="eventData"></param>
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;

        /// <summary>
        ///     触发一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        /// <summary>
        ///     触发一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="eventData"></param>
        void Trigger(Type eventType, IEventData eventData);

        /// <summary>
        ///     触发一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        void Trigger(Type eventType, object eventSource, IEventData eventData);

        /// <summary>
        ///     异步触发一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     Event type
        ///     <para>事件类型</para>
        /// </typeparam>
        /// <param name="eventData"></param>
        /// <returns>The task to handle async operation</returns>
        Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData;

        /// <summary>
        ///     异步触发一个事件.
        /// </summary>
        /// <typeparam name="TEventData">
        ///     事件类型
        /// </typeparam>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        /// <returns>The task to handle async operation</returns>
        Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData;

        /// <summary>
        ///     异步触发一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="eventData"></param>
        /// <returns>The task to handle async operation</returns>
        Task TriggerAsync(Type eventType, IEventData eventData);

        /// <summary>
        ///     异步触发一个事件.
        /// </summary>
        /// <param name="eventType">
        ///     事件类型
        /// </param>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        /// <returns>The task to handle async operation</returns>
        Task TriggerAsync(Type eventType, object eventSource, IEventData eventData);

        #endregion
    }
}
