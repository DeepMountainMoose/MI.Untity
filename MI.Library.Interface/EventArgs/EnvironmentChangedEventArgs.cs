using MI.Library.Interface.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.EventArgs
{
    public class EnvironmentChangedEventArgs : System.EventArgs
    {
        /// <summary>
        ///     修改前的环境
        /// </summary>
        public EnvironmentType OldEvironment { get; }

        /// <summary>
        ///     修改后的环境
        /// </summary>
        public EnvironmentType NewEnvironment { get; }

        public EnvironmentChangedEventArgs(EnvironmentType oldEvironment, EnvironmentType newEnvironment)
        {
            OldEvironment = oldEvironment;
            NewEnvironment = newEnvironment;
        }
    }
}
