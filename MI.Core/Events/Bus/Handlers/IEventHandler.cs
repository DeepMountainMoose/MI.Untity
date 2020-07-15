using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Events.Bus.Handlers
{
    /// <summary>
    ///     事件EventHandler的基类.
    ///     <para>通过实现 <see cref="IEventHandler{TEventData}" /> 代替此类</para>
    /// </summary>
    public interface IEventHandler
    {
    }

    /// <summary>
    ///     定义一个处理事件数据 <typeparamref name="TEventData" /> 的EventHandler接口.
    /// </summary>
    /// <typeparam name="TEventData">Event type to handle</typeparam>
    public interface IEventHandler<in TEventData> : IEventHandler
    {
        /// <summary>
        ///     处理事件的方法.
        /// </summary>
        /// <param name="eventData">Event data</param>
        void HandleEvent(TEventData eventData);
    }
}
