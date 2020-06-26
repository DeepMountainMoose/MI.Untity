using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Library.Interface.Attribute
{
    /// <summary>
    ///     可选参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExtraSwaggerParameter : System.Attribute
    {
        /// <summary>
        ///     参数
        /// </summary>
        public string[] ParameterNames { get; }

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="parameterNames"></param>
        public ExtraSwaggerParameter(params string[] parameterNames)
        {
            ParameterNames = parameterNames;
        }

        public const string ChannelId = "ChannelId";

        public const string EnterpriseId = "EnterpriseId";

        public const string Command = "Command";

        public const string Remark = "Remark";
    }
}
