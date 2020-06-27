using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Common
{
    /// <summary>
    ///     环境配置
    /// </summary>
    public static class EnvironmentConfiguration
    {
        private static IEnvironmentProvider _environmentProvider;

        public static void SetEnvironmentProviderFunc(IEnvironmentProvider environmentProvider)
        {
            _environmentProvider = environmentProvider;
        }

        /// <summary>
        ///     当前环境配置
        /// </summary>
        public static EnvironmentType Environment => _environmentProvider?.GetCurrentEnvironment() ?? EnvironmentType.Default;
    }
}
