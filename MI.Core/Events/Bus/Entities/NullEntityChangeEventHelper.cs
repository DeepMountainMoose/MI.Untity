using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MI.Core.Events.Bus.Entities
{
    public class NullEntityChangeEventHelper : IEntityChangeEventHelper
    {
        /// <summary>
        /// 获取<see cref="NullEntityChangeEventHelper"/>的空引用实现.
        /// </summary>
        public static NullEntityChangeEventHelper Instance { get; } = new NullEntityChangeEventHelper();

        public void TriggerEvents(EntityChangeReport changeReport)
        {
            //nothing
        }

        public Task TriggerEventsAsync(EntityChangeReport changeReport)
        {
            return Task.FromResult(0);
        }
    }
}
