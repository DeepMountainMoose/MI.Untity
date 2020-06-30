using MI.Library.EventArgs;
using MI.Library.Interface.Enum;
using System;

namespace MI.Library.Interface
{
    /// <summary>
    ///     环境变量提供程序
    /// </summary>
    public interface IEnvironmentProvider
    {
        EnvironmentType GetCurrentEnvironment();

        /// <summary>
        ///     环境发生变化后触发的事件
        /// </summary>
        event EventHandler<EnvironmentChangedEventArgs> OnEnvironmentChanged;

        /// <summary>
        ///     设置环境变量
        ///     <remarks>一般情况下环境变量应该由系统自己初始化处理,此方法应该仅用于测试</remarks>
        /// </summary>
        void SetEnvironment(EnvironmentType environmentType);
    }
}
