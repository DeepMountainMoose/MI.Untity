using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface
{
    [Flags]
    public enum StartupModeType
    {
        None = 0,
        /// <summary>
        ///     接口模式
        /// </summary>
        Api = 1,
        /// <summary>
        ///     前台Ui模式
        /// </summary>
        Ui = 2,
        /// <summary>
        ///     管理站点模式
        /// </summary>
        Admin = 4,

        /// <summary>
        ///     所有模式
        /// </summary>
        All = Admin | Api | Ui
    }
}
