using MI.Core.Events.Bus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Events.Bus.Factories
{
    /// <summary>
    ///     定义一个接口,通过工厂模式创建/释放事件处理器.
    /// </summary>
    public interface IEventHandlerFactory
    {
        /// <summary>
        ///     返回事件处理器.
        /// </summary>
        /// <returns>当前事件工厂生成的事件处理器实例</returns>
        IEventHandler GetHandler();

        /// <summary>
        ///     获取事件处理器的类型
        /// </summary>
        /// <returns></returns>
        Type GetHandlerType();

        /// <summary>
        ///     释放事件处理器.
        /// </summary>
        /// <param name="handler">释放处理器</param>
        void ReleaseHandler(IEventHandler handler);
    }
}
