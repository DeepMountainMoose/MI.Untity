using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Component.Core.Exceptions
{
    public static class ParameterChecker
    {
        /// <summary>
        /// 判断实体对象是否为NULL
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        public static void CheckNull(object parameter, string parameterName)
        {
            ParameterChecker.CheckParameterName(parameterName);
            if (parameter == null)
            {
                throw new MIParameterException($"对象 {parameterName} 为NULL");
            }
        }

        public static void CheckNullOrEmpty(Guid? parameter, string parameterName)
        {
            ParameterChecker.CheckParameterName(parameterName);
            if (!parameter.HasValue || parameter.Value == Guid.Empty)
            {
                throw new MIParameterException($"Guid {parameterName} 为NULL或者为空");
            }
        }

        public static void CheckEmpty(Guid parameter, string parameterName)
        {
            ParameterChecker.CheckParameterName(parameterName);
            if (parameter == Guid.Empty)
            {
                throw new MIParameterException(string.Format("Guid {0} 为空", parameterName));
            }
        }

        public static void CheckNullOrEmpty(string parameter, string parameterName)
        {
            ParameterChecker.CheckParameterName(parameterName);
            if (string.IsNullOrEmpty(parameter))
            {
                throw new MIParameterException($"字符串 {parameterName} 为NULL或者为空");
            }
        }

        public static void CheckNullOrWhiteSpace(string parameter, string parameterName)
        {
            ParameterChecker.CheckParameterName(parameterName);
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new MIParameterException($"字符串 {parameterName} 为NULL或者为空或者只包含字符串");
            }
        }


        public static void CheckParameterName(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new MIParameterException($"{parameterName}为不能为NULL或者string.Empty");
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new MIParameterException($"{parameterName}不能为空字符串");
            }
        }
    }
}
