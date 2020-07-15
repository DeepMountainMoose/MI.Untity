using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Events.Bus
{
    /// <summary>
    ///     定义用于所有事件的数据接口
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        ///     事件触发时间
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        ///     触发事件的对象(事件源)
        /// </summary>
        object EventSource { get; set; }
    }
}
