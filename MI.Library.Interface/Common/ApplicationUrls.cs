using MI.Library.Interface.Enum;
using System;

namespace MI.Library.Interface.Common
{
    /// <summary>
    ///     应用程序地址
    /// </summary>
    public static class ApplicationUrls
    {
        private static IApplicationUrlProvider _provider;
        public static IApplicationUrlProvider Provider
        {
            get => _provider;
            set => _provider = value ?? throw new ArgumentNullException(nameof(value), "Can not set ApplicationUrls.Provider to null");
        }

        /// <summary>
        ///     EhiInterface后台地址
        /// <remarks>对应线上:http://interface.1hai.cn/api/ </remarks>
        /// </summary>
        public static string InterfaceUrl => Provider?.GetUrl(ApplicationType.Interface) ?? "http://interface.dev.ehi.com.cn/";
    }
}
