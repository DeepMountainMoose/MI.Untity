using MI.Library.Interface.Common;
using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Common
{
    /// <summary>
    ///     常量配置
    /// </summary>
    public static class Const
    {
        /// <summary>
        ///     异常的时候记录异常站点的Header
        /// </summary>
        public const string ErrorSiteOriginPlatformHeader = "x-ehi-ex-platform";

        public const string ExternalSignHeader = "x-ehi-sign";
        public const string 系统错误话术 = "网络或程序异常，请稍后再试";
        public const string 未知话术 = "未知话术";

        internal const string JwtIssuer = "eHi";

        public const string LoginCookiesName = "elc";

        public const string ReturnUrlName = "rdu";

        public const string LoginSiteName = "lsn";

        internal const string JwtKey = "AgQGCAoMDfASFAIEBggKDA4QETAdBAYICgwOE52UAgQ=";

        internal const string JwtSubKey = "sub";

        internal const string JwtEnterpriseId = "entid";

        internal static string QuanxianUrl { get; private set; }

        internal static string FileUrl { get; private set; }


    }
}
