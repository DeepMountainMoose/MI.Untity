using System.Threading.Tasks;

namespace MI.Core.Events.Bus.Entities
{
    /// <summary>
    ///     用于触发实体变更事件的帮助类.
    /// </summary>
    public interface IEntityChangeEventHelper
    {
        void TriggerEvents(EntityChangeReport changeReport);

        Task TriggerEventsAsync(EntityChangeReport changeReport);

    }
}
