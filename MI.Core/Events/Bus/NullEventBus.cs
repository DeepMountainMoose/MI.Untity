using MI.Core.Events.Bus.Factories;
using MI.Core.Events.Bus.Handlers;
using MI.Core.Util;
using System;
using System.Threading.Tasks;

namespace MI.Core.Events.Bus
{
    /// <summary>
    ///     空引用模式的Event Bus实现.
    /// </summary>
    public sealed class NullEventBus : IEventBus
    {
        private NullEventBus()
        {
        }

        /// <summary>
        ///     <see cref="NullEventBus" /> 的单例模式实例.
        /// </summary>
        public static NullEventBus Instance { get; } = new NullEventBus();

        /// <inheritdoc />
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable AsyncRegister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc />
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Unregister<TEventData>(IEventHandler<TEventData> handler) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Unregister(Type eventType, IEventHandler handler)
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc/>
        public void AsyncUnregister<TEventData>(Func<TEventData, Task> action) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void UnregisterAll(Type eventType)
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, IEventData eventData)
        {
            //Do nothing
        }

        /// <inheritdoc />
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            //Do nothing
        }

        /// <inheritdoc />
        public Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task TriggerAsync<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task TriggerAsync(Type eventType, IEventData eventData)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task TriggerAsync(Type eventType, object eventSource, IEventData eventData)
        {
            return Task.CompletedTask;
        }
    }
}
