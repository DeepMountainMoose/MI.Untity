using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MI.Core.Extensions
{
    /// <summary>
    ///     字符串的扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     指示该字符串是否为空或者是<see cref="System.String.Empty" />
        /// </summary>
        [ContractAnnotation("str:null => true")]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        ///     指示该字符串是否为空或者是System.String.Empty, 或者是纯空白字符
        /// </summary>
        [ContractAnnotation("str:null => true")]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        ///     从字符串起始位置开始截取字符串
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="str" /> 为null的时候抛出异常
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     当 <paramref name="len" /> 大于字符串长度的时候抛出异常
        /// </exception>
        public static string Left(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str.Substring(0, len);
        }

        /// <summary>
        ///     从字符串结束位置开始截取字符串
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="str" /> 为null的时候抛出异常
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     当 <paramref name="len" /> 大于字符串长度的时候抛出异常
        /// </exception>
        public static string Right(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 从指定字符串中移除第一个匹配的前缀.
        /// 顺序很重要,当第一个前缀被匹配到后不会再匹配其它前缀.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="preFixes"></param>
        /// <returns></returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (preFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var preFix in preFixes)
            {
                if (str.StartsWith(preFix))
                {
                    return str.Right(str.Length - preFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// 从指定字符串中移除第一个匹配的后缀.
        /// 顺序很重要,当第一个后缀被匹配到后不会再匹配其它后缀.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="postFixes"></param>
        /// <returns></returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Left(str.Length - postFix.Length);
                }
            }

            return str;
        }
    }
}
